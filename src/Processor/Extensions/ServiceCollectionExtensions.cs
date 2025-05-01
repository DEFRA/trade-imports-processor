using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.Errors;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Utils.Logging;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Interceptor;
using SlimMessageBus.Host.Serialization.SystemTextJson;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class ServiceCollectionExtensions
{
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
        services.AddSlimMessageBus(smb =>
        {
            smb.AddChildBus(
                "ASB_Notification",
                mbb =>
                {
                    var serviceBusOptions = services
                        .AddValidateOptions<ServiceBusOptions>(configuration, ServiceBusOptions.SectionName)
                        .Get();

                    mbb.WithProviderServiceBus(cfg =>
                    {
                        cfg.TopologyProvisioning = new ServiceBusTopologySettings { Enabled = false };
                        cfg.ClientFactory = (sp, settings) =>
                        {
                            var clientOptions = sp.GetRequiredService<IHostEnvironment>().IsDevelopment()
                                ? new ServiceBusClientOptions()
                                : new ServiceBusClientOptions
                                {
                                    WebProxy = sp.GetRequiredService<IWebProxy>(),
                                    TransportType = ServiceBusTransportType.AmqpWebSockets,
                                };

                            return new ServiceBusClient(settings.ConnectionString, clientOptions);
                        };
                        cfg.ConnectionString = serviceBusOptions.Notifications.ConnectionString;
                    });
                    mbb.AddJsonSerializer();

                    mbb.AddServicesFromAssemblyContaining<NotificationConsumer>();
                    mbb.Consume<JsonElement>(x =>
                    {
                        x.Topic(serviceBusOptions.Notifications.Topic)
                            .SubscriptionName(serviceBusOptions.Notifications.Subscription)
                            .WithConsumer<NotificationConsumer>()
                            .Instances(1);
                    });
                }
            );

            smb.AddChildBus(
                "SQS_CustomsDeclarations",
                mbb =>
                {
                    var customsDeclarationsConsumerOptions = services
                        .AddValidateOptions<CustomsDeclarationsConsumerOptions>(
                            configuration,
                            CustomsDeclarationsConsumerOptions.SectionName
                        )
                        .Get();

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
                            .Instances(1)
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

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ClearanceRequestValidatorInput>, ClearanceRequestValidator>();
        services.AddScoped<IValidator<CustomsDeclarationsMessage>, CustomsDeclarationsMessageValidator>();
        services.AddScoped<IValidator<ErrorNotification>, ErrorNotificationValidator>();
        services.AddScoped<IValidator<FinalisationValidatorInput>, FinalisationValidator>();

        return services;
    }
}
