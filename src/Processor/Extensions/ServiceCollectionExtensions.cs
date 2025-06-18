using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Utils.CorrelationId;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.Gmrs;
using FluentValidation;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Interceptor;
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

        // Order of interceptors is important here
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(TraceContextInterceptor<>));
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(LoggingInterceptor<>));
        services.AddSingleton<ConsumerMetrics>();
        services.AddSingleton(typeof(IConsumerInterceptor<>), typeof(MetricsInterceptor<>));

        services.AddSlimMessageBus(smb =>
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
                    mbb.Consume<JsonElement>(x =>
                    {
                        x.Topic(serviceBusOptions.Gmrs.Topic)
                            .SubscriptionName(serviceBusOptions.Gmrs.Subscription)
                            .WithConsumer<GmrsConsumer>()
                            .Instances(serviceBusOptions.Gmrs.ConsumersPerHost);
                    });
                }
            );

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
                    mbb.Consume<JsonElement>(x =>
                    {
                        x.Topic(serviceBusOptions.Notifications.Topic)
                            .SubscriptionName(serviceBusOptions.Notifications.Subscription)
                            .WithConsumer<NotificationConsumer>()
                            .Instances(serviceBusOptions.Notifications.ConsumersPerHost);
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
                            .Instances(customsDeclarationsConsumerOptions.ConsumersPerHost)
                    );
                }
            );
        });

        // Concrete consumers added for temporary replay endpoints
        services.AddTransient<NotificationConsumer>();
        services.AddTransient<CustomsDeclarationsConsumer>();

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
