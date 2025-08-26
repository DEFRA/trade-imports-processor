using System.Diagnostics.CodeAnalysis;
using System.Net;
using Azure.Core.Pipeline;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Utils.Http;
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

    public static IHealthChecksBuilder AddAsbTopic(
        this IHealthChecksBuilder builder,
        string name,
        Func<IServiceProvider, ServiceBusPublisherOptions> publisherFunc,
        HealthStatus? failureStatus = HealthStatus.Unhealthy,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null
    )
    {
        builder.Add(
            new HealthCheckRegistration(
                name,
                sp => CreateHealthCheck(sp, publisherFunc(sp)),
                failureStatus,
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

    private static AzureServiceBusTopicHealthCheck CreateHealthCheck(
        IServiceProvider serviceProvider,
        ServiceBusPublisherOptions publisher
    )
    {
        var options = new AzureServiceBusTopicHealthCheckOptions(publisher.Topic)
        {
            ConnectionString = publisher.ConnectionString,
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

        public override ServiceBusAdministrationClient CreateManagementClient(string? connectionString)
        {
            var clientOptions = !serviceProvider.GetRequiredService<IOptions<CdpOptions>>().Value.IsProxyEnabled
                ? new ServiceBusAdministrationClientOptions()
                : new ServiceBusAdministrationClientOptions
                {
                    Transport = new HttpClientTransport(
                        serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(Proxy.ProxyClient)
                    ),
                };

            clientOptions.Retry.MaxRetries = 0;

            return new ServiceBusAdministrationClient(connectionString, clientOptions);
        }
    }
}
