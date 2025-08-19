using System.Diagnostics.CodeAnalysis;
using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using HealthChecks.AzureServiceBus;
using HealthChecks.AzureServiceBus.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Health;

[ExcludeFromCodeCoverage]
public static class AsbHealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddAsbTopic(
        this IHealthChecksBuilder builder,
        string name,
        Func<IServiceProvider, ServiceBusSubscriptionOptions> subscriptionFunc,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null
    )
    {
        builder.Add(
            new HealthCheckRegistration(
                name,
                sp => CreateHealthCheck(sp, subscriptionFunc(sp)),
                HealthStatus.Unhealthy,
                tags,
                timeout
            )
        );

        return builder;
    }

    public static IHealthChecksBuilder AddAsbPublishTopic(
        this IHealthChecksBuilder builder,
        string name,
        Func<IServiceProvider, ServiceBusPublisherOptions> serviceBusOptionsFunc,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null
    )
    {
        builder.Add(
            new HealthCheckRegistration(
                name,
                sp => CreatePublishHealthCheck(sp, serviceBusOptionsFunc(sp)),
                HealthStatus.Unhealthy,
                tags,
                timeout
            )
        );

        return builder;
    }

    private static AzureServiceBusSubscriptionHealthCheck CreateHealthCheck(
        IServiceProvider serviceProvider,
        ServiceBusSubscriptionOptions subscription
    )
    {
        var options = new AzureServiceBusSubscriptionHealthCheckHealthCheckOptions(
            subscription.Topic,
            subscription.Subscription
        )
        {
            ConnectionString = subscription.ConnectionString,
            UsePeekMode = true,
        };

        return new AzureServiceBusSubscriptionHealthCheck(options, new ServiceBusClientProvider(serviceProvider));
    }

    private static AzureServiceBusTopicHealthCheck CreatePublishHealthCheck(
        IServiceProvider serviceProvider,
        ServiceBusPublisherOptions serviceBusPublisherOptions
    )
    {
        var options = new AzureServiceBusTopicHealthCheckOptions(serviceBusPublisherOptions.Topic)
        {
            ConnectionString = serviceBusPublisherOptions.ConnectionString,
        };

        return new AzureServiceBusTopicHealthCheck(options, new ServiceBusClientProvider(serviceProvider));
    }

    private sealed class ServiceBusClientProvider(IServiceProvider serviceProvider)
        : HealthChecks.AzureServiceBus.ServiceBusClientProvider
    {
        public override ServiceBusClient CreateClient(string? connectionString)
        {
            var clientOptions = !serviceProvider.GetRequiredService<IOptions<CdpOptions>>().Value.IsProxyEnabled
                ? new ServiceBusClientOptions()
                : new ServiceBusClientOptions
                {
                    WebProxy = serviceProvider.GetRequiredService<IWebProxy>(),
                    TransportType = ServiceBusTransportType.AmqpWebSockets,
                };

            return new ServiceBusClient(connectionString, clientOptions);
        }
    }
}
