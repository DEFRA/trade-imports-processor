using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Services;
using Defra.TradeImportsProcessor.Processor.Utils;
using Defra.TradeImportsProcessor.Processor.Utils.CorrelationId;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.Gmrs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Interceptor;
using SlimMessageBus.Host.Serialization;
using SlimMessageBus.Host.Serialization.SystemTextJson;
using Gmr = Defra.TradeImportsDataApi.Domain.Gvms.Gmr;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataApiHttpClient(this IServiceCollection services)
    {
        var resilienceOptions = new HttpStandardResilienceOptions { Retry = { UseJitter = true } };
        resilienceOptions.Retry.DisableForUnsafeHttpMethods();

        services
            .AddTradeImportsDataApiClient()
            .ConfigureHttpClient(
                (sp, c) =>
                {
                    sp.GetRequiredService<IOptions<DataApiOptions>>().Value.Configure(c);

                    // Disable the HttpClient timeout to allow the resilient pipeline below
                    // to handle all timeouts
                    c.Timeout = Timeout.InfiniteTimeSpan;
                }
            )
            .AddHeaderPropagation()
            .AddResilienceHandler(
                "DataApi",
                builder =>
                {
                    builder
                        .AddTimeout(resilienceOptions.TotalRequestTimeout)
                        .AddRetry(resilienceOptions.Retry)
                        .AddTimeout(resilienceOptions.AttemptTimeout);
                }
            );

        return services;
    }

    public static IServiceCollection AddProcessorConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
        services.AddOptions<CdpOptions>().Bind(configuration).ValidateDataAnnotations();
        services.AddOptions<DataApiOptions>().BindConfiguration(DataApiOptions.SectionName).ValidateDataAnnotations();

        return services;
    }

    public static IServiceCollection AddCustomMetrics(this IServiceCollection services)
    {
        services.AddTransient<MetricsMiddleware>();

        services.AddSingleton<IConsumerMetrics, ConsumerMetrics>();
        services.AddSingleton<RequestMetrics>();
        services.AddSingleton<AzureMetrics>();

        services.AddAzureDeadLetterPolling(nameof(ImportNotification), options => options.Notifications);
        services.AddAzureDeadLetterPolling(nameof(Gmr), options => options.Gmrs);

        return services;
    }

    private static IServiceCollection AddAzureDeadLetterPolling(
        this IServiceCollection services,
        string consumerName,
        Func<ServiceBusOptions, ServiceBusSubscriptionOptions> resolve
    )
    {
        services.AddKeyedSingleton<IDeadLetterService, ServiceBusDeadLetterService>(
            consumerName,
            (sp, _) =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

                return ActivatorUtilities.CreateInstance<ServiceBusDeadLetterService>(sp, resolve(options));
            }
        );
        services.Add(
            ServiceDescriptor.Singleton<IHostedService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

                return ActivatorUtilities.CreateInstance<AzureDeadLetterBackgroundService>(
                    sp,
                    resolve(options),
                    consumerName,
                    sp.GetRequiredKeyedService<IDeadLetterService>(consumerName)
                );
            })
        );

        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        var customsDeclarationsConsumerOptions = services
            .AddValidateOptions<CustomsDeclarationsConsumerOptions>(CustomsDeclarationsConsumerOptions.SectionName)
            .Get();
        var resourceEventsConsumerOptions = services
            .AddValidateOptions<ResourceEventsConsumerOptions>(ResourceEventsConsumerOptions.SectionName)
            .Get();
        var serviceBusOptions = services.AddValidateOptions<ServiceBusOptions>(ServiceBusOptions.SectionName).Get();
        var rawMessageLoggingOptions = services
            .AddValidateOptions<RawMessageLoggingOptions>(RawMessageLoggingOptions.SectionName)
            .Get();

        // Order of interceptors is important here
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(TraceContextInterceptor<>));
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(LoggingInterceptor<>));
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(MetricsInterceptor<>));

        if (rawMessageLoggingOptions.Enabled)
            services.AddScoped(typeof(IConsumerInterceptor<>), typeof(RawMessageLoggingInterceptor<>));

        services.AddTransient(typeof(IServiceBusConsumerErrorHandler<>), typeof(AzureConsumerErrorHandler<>));

        services.AddSlimMessageBus(smb =>
        {
            if (serviceBusOptions.Gmrs.AutoStartConsumers)
            {
                smb.AddChildBus(
                    "ASB_Gmrs",
                    mbb =>
                    {
                        mbb.WithProviderServiceBus(
                            CdpServiceBusClientFactory.ConfigureServiceBus(
                                serviceBusOptions.Gmrs.ConnectionString,
                                serviceBusOptions.Gmrs.ConsumersPerHost
                            )
                        );
                        mbb.AddJsonSerializer();
                        mbb.AddServicesFromAssemblyContaining<GmrsConsumer>();
                        mbb.AutoStartConsumersEnabled(serviceBusOptions.Gmrs.AutoStartConsumers)
                            .Consume<JsonElement>(x =>
                            {
                                x.Topic(serviceBusOptions.Gmrs.Topic)
                                    .SubscriptionName(serviceBusOptions.Gmrs.Subscription)
                                    .WithConsumer<GmrsConsumer>()
                                    .Instances(serviceBusOptions.Gmrs.ConsumersPerHost);
                            });
                    }
                );
            }

            if (serviceBusOptions.Notifications.AutoStartConsumers)
            {
                smb.AddChildBus(
                    "ASB_Notification",
                    mbb =>
                    {
                        mbb.WithProviderServiceBus(
                            CdpServiceBusClientFactory.ConfigureServiceBus(
                                serviceBusOptions.Notifications.ConnectionString,
                                serviceBusOptions.Notifications.ConsumersPerHost
                            )
                        );
                        mbb.AddJsonSerializer();
                        mbb.AutoStartConsumersEnabled(serviceBusOptions.Notifications.AutoStartConsumers)
                            .Consume<JsonElement>(x =>
                            {
                                x.Topic(serviceBusOptions.Notifications.Topic)
                                    .SubscriptionName(serviceBusOptions.Notifications.Subscription)
                                    .WithConsumer<NotificationConsumer>()
                                    .Instances(serviceBusOptions.Notifications.ConsumersPerHost);
                            });
                    }
                );
            }

            if (customsDeclarationsConsumerOptions.AutoStartConsumers)
            {
                smb.AddChildBus(
                    "SQS_CustomsDeclarations",
                    mbb =>
                    {
                        mbb.WithProviderAmazonSQS(cfg =>
                        {
                            cfg.TopologyProvisioning.Enabled = false;
                            cfg.ClientProviderFactory = _ => new CdpCredentialsSqsClientProvider(
                                cfg.SqsClientConfig,
                                configuration
                            );
                        });
                        mbb.AddJsonSerializer();
                        mbb.AddServicesFromAssemblyContaining<CustomsDeclarationsConsumer>();
                        mbb.AutoStartConsumersEnabled(customsDeclarationsConsumerOptions.AutoStartConsumers)
                            .Consume<JsonElement>(x =>
                                x.WithConsumer<CustomsDeclarationsConsumer>()
                                    .Queue(customsDeclarationsConsumerOptions.QueueName)
                                    .Instances(customsDeclarationsConsumerOptions.ConsumersPerHost)
                            );
                    }
                );
            }

            if (resourceEventsConsumerOptions.AutoStartConsumers)
            {
                smb.AddChildBus(
                    "SQS_ResourceEvents",
                    mbb =>
                    {
                        mbb.WithProviderAmazonSQS(cfg =>
                        {
                            cfg.TopologyProvisioning.Enabled = false;
                            cfg.ClientProviderFactory = _ => new CdpCredentialsSqsClientProvider(
                                cfg.SqsClientConfig,
                                configuration
                            );
                        });

                        mbb.RegisterSerializer<ToStringSerializer>(s =>
                        {
                            s.TryAddSingleton(_ => new ToStringSerializer());
                            s.TryAddSingleton<IMessageSerializer<string>>(svp =>
                                svp.GetRequiredService<ToStringSerializer>()
                            );
                        });

                        mbb.WithSerializer<ToStringSerializer>();

                        mbb.AutoStartConsumersEnabled(resourceEventsConsumerOptions.AutoStartConsumers)
                            .Consume<string>(x =>
                                x.WithConsumer<ResourceEventsConsumer>()
                                    .Queue(resourceEventsConsumerOptions.QueueName)
                                    .Instances(resourceEventsConsumerOptions.ConsumersPerHost)
                            );
                    }
                );
            }
        });

        // Concrete consumers added for temporary replay endpoints
        services.AddTransient<NotificationConsumer>();
        services.AddTransient<CustomsDeclarationsConsumer>();

        services.AddPublishers(serviceBusOptions);

        return services;
    }

    private static IServiceCollection AddPublishers(
        this IServiceCollection services,
        ServiceBusOptions serviceBusOptions
    )
    {
        var btmsOptions = services
            .AddOptions<BtmsOptions>()
            .BindConfiguration(BtmsOptions.SectionName)
            .ValidateDataAnnotations()
            .Get();

        if (btmsOptions.OperatingMode == OperatingMode.Cutover)
        {
            services.AddScoped<IIpaffsStrategy, DecisionNotificationStrategy>();

            services.AddSlimMessageBus(smb =>
            {
                smb.AddChildBus(
                    "ASB_IpaffsPublisher",
                    mbb =>
                    {
                        mbb.WithProviderServiceBus(cfg =>
                        {
                            cfg.ConnectionString = serviceBusOptions.Ipaffs.ConnectionString;
                            cfg.TopologyProvisioning.Enabled = false;
                        });
                        mbb.AddJsonSerializer();
                        mbb.Produce<DecisionNotification>(x =>
                            x.DefaultTopic(serviceBusOptions.Ipaffs.Topic)
                                .WithModifier(
                                    (message, sbMessage) =>
                                    {
                                        sbMessage.ApplicationProperties.Remove("MessageType");
                                    }
                                )
                        );
                    }
                );
            });
        }

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ClearanceRequestValidatorInput>, ClearanceRequestValidator>();
        services.AddScoped<IValidator<CustomsDeclarationsMessage>, CustomsDeclarationsMessageValidator>();
        services.AddScoped<IValidator<ExternalError>, ErrorNotificationValidator>();
        services.AddScoped<IValidator<FinalisationValidatorInput>, FinalisationValidator>();
        services.AddScoped<IValidator<Gmr>, GmrValidator>();

        return services;
    }
}
