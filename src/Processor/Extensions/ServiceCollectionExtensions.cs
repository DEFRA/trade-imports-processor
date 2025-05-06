using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.Errors;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.Gmrs;
using FluentValidation;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Interceptor;
using SlimMessageBus.Host.Serialization.SystemTextJson;
using Gmr = Defra.TradeImportsDataApi.Domain.Gvms.Gmr;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class ServiceCollectionExtensions
{
    private const int ConsumersInstanceCount = 20;

    public static IServiceCollection AddDataApiHttpClient(this IServiceCollection services)
    {
        services
            .AddTradeImportsDataApiClient()
            .ConfigureHttpClient(
                (sp, c) =>
                {
                    var options = sp.GetRequiredService<IOptions<DataApiOptions>>().Value;
                    c.BaseAddress = new Uri(options.BaseAddress);

                    if (options.BasicAuthCredential != null)
                        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            options.BasicAuthCredential
                        );

                    if (c.BaseAddress.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                        c.DefaultRequestVersion = HttpVersion.Version20;
                }
            )
            .AddHeaderPropagation()
            .AddStandardResilienceHandler(o =>
            {
                o.Retry.DisableForUnsafeHttpMethods();
            });

        return services;
    }

    public static IServiceCollection AddProcessorConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddOptions<CdpOptions>().Bind(configuration).ValidateDataAnnotations();
        services.AddOptions<DataApiOptions>().BindConfiguration(DataApiOptions.SectionName).ValidateDataAnnotations();

        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        var customsDeclarationsConsumerOptions = services
            .AddValidateOptions<CustomsDeclarationsConsumerOptions>(
                configuration,
                CustomsDeclarationsConsumerOptions.SectionName
            )
            .Get();
        var serviceBusOptions = services
            .AddValidateOptions<ServiceBusOptions>(configuration, ServiceBusOptions.SectionName)
            .Get();

        services.AddSlimMessageBus(smb =>
        {
            smb.AddChildBus(
                "ASB_Gmrs",
                mbb =>
                {
                    mbb.WithProviderServiceBus(
                        CdpServiceBusClientFactory.ConfigureServiceBus(serviceBusOptions.Gmrs.ConnectionString, ConsumersInstanceCount)
                    );
                    mbb.AddJsonSerializer();
                    mbb.AddServicesFromAssemblyContaining<GmrsConsumer>();
                    mbb.Consume<JsonElement>(x =>
                    {
                        x.Topic(serviceBusOptions.Gmrs.Topic)
                            .SubscriptionName(serviceBusOptions.Gmrs.Subscription)
                            .WithConsumer<GmrsConsumer>()
                            .Instances(ConsumersInstanceCount);
                    });
                }
            );

            smb.AddChildBus(
                "ASB_Notification",
                mbb =>
                {
                    mbb.WithProviderServiceBus(
                        CdpServiceBusClientFactory.ConfigureServiceBus(serviceBusOptions.Notifications.ConnectionString, ConsumersInstanceCount)
                    );
                    mbb.AddJsonSerializer();

                    mbb.AddServicesFromAssemblyContaining<NotificationConsumer>();
                    mbb.Consume<JsonElement>(x =>
                    {
                        x.Topic(serviceBusOptions.Notifications.Topic)
                            .SubscriptionName(serviceBusOptions.Notifications.Subscription)
                            .WithConsumer<NotificationConsumer>()
                            .Instances(ConsumersInstanceCount);
                    });
                }
            );

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
                    mbb.Consume<JsonElement>(x =>
                        x.WithConsumer<CustomsDeclarationsConsumer>()
                            .Queue(customsDeclarationsConsumerOptions.QueueName)
                            .Instances(ConsumersInstanceCount)
                    );
                }
            );
        });

        return services;
    }

    public static IServiceCollection AddTracingForConsumers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IConsumerInterceptor<>), typeof(TraceContextInterceptor<>));
        services.AddSingleton(typeof(IServiceBusConsumerErrorHandler<>), typeof(SerilogTraceErrorHandler<>));

        return services;
    }

    public static IServiceCollection AddMetricsForConsumers(this IServiceCollection services)
    {
        services.AddSingleton<ConsumerMetrics>();
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(MetricsInterceptor<>));

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ClearanceRequestValidatorInput>, ClearanceRequestValidator>();
        services.AddScoped<IValidator<CustomsDeclarationsMessage>, CustomsDeclarationsMessageValidator>();
        services.AddScoped<IValidator<ErrorNotification>, ErrorNotificationValidator>();
        services.AddScoped<IValidator<FinalisationValidatorInput>, FinalisationValidator>();
        services.AddScoped<IValidator<Gmr>, GmrValidator>();

        return services;
    }
}
