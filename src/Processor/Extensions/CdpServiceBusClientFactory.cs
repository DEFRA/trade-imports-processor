using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class CdpServiceBusClientFactory
{
    private static ServiceBusClient ConfigureServiceBusClient(
        IServiceProvider serviceProvider,
        ServiceBusMessageBusSettings busSettings
    )
    {
        var clientOptions = !serviceProvider.GetRequiredService<IOptions<CdpOptions>>().Value.IsProxyEnabled
            ? new ServiceBusClientOptions()
            : new ServiceBusClientOptions
            {
                WebProxy = serviceProvider.GetRequiredService<IWebProxy>(),
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };

        return new ServiceBusClient(busSettings.ConnectionString, clientOptions);
    }

    public static Action<ServiceBusMessageBusSettings> ConfigureServiceBus(string connectionString, int instanceCount)
    {
        return settings =>
        {
            settings.PrefetchCount = instanceCount;
            settings.TopologyProvisioning.Enabled = false;
            settings.ClientFactory = ConfigureServiceBusClient;
            settings.ConnectionString = connectionString;
        };
    }
}
