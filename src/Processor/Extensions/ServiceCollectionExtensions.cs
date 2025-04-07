using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;
using SlimMessageBus.Host.Serialization.SystemTextJson;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSlimMessageBus(mbb =>
        {
            var serviceBusOptions = services
                .AddValidateOptions<ServiceBusOptions>(configuration, ServiceBusOptions.SectionName)
                .Get();

            mbb.AddChildBus(
                "ASB_Notification",
                cbb =>
                {
                    ConfigureServiceBusClient(cbb, serviceBusOptions.Notifications.ConnectionString);

                    cbb.AddServicesFromAssemblyContaining<NotificationConsumer>()
                        .Consume<Dictionary<string, object>>(x =>
                            x.Topic(serviceBusOptions.Notifications.Topic)
                                .SubscriptionName(serviceBusOptions.Notifications.Subscription)
                                .WithConsumer<NotificationConsumer>()
                                .Instances(20)
                        );
                }
            );
        });

        return services;
    }

    private static void ConfigureServiceBusClient(MessageBusBuilder cbb, string connectionString)
    {
        cbb.WithProviderServiceBus(cfg =>
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
            cfg.ConnectionString = connectionString;
        });
        cbb.AddJsonSerializer();
    }
}
