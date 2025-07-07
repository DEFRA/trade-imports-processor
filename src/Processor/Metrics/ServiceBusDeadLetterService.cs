using System.Diagnostics.CodeAnalysis;
using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class ServiceBusDeadLetterService : IDeadLetterService, IAsyncDisposable
{
    private readonly ServiceBusSubscriptionOptions _options;
    private readonly IOptions<CdpOptions> _cdpOptions;
    private readonly IWebProxy _webProxy;
    private readonly ServiceBusClient _client;

    public ServiceBusDeadLetterService(
        ServiceBusSubscriptionOptions options,
        IOptions<CdpOptions> cdpOptions,
        IWebProxy webProxy
    )
    {
        _options = options;
        _cdpOptions = cdpOptions;
        _webProxy = webProxy;

        _client = CreateClient();
    }

    private ServiceBusClient CreateClient()
    {
        var clientOptions = !_cdpOptions.Value.IsProxyEnabled
            ? new ServiceBusClientOptions()
            : new ServiceBusClientOptions
            {
                WebProxy = _webProxy,
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            };

        return new ServiceBusClient(_options.ConnectionString, clientOptions);
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task<int> PeekTotalMessageCount(CancellationToken cancellationToken)
    {
        // A long-lived receiver ie. singleton does not work for repeated calls
        // due to the inner implementation of AMQP. Therefore, create a new receiver
        // for each call
        await using var receiver = _client.CreateReceiver(
            _options.Topic,
            _options.Subscription,
            new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
            }
        );

        return await receiver.PeekTotalMessageCount(cancellationToken: cancellationToken);
    }
}
