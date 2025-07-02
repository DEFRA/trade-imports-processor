using System.Diagnostics.CodeAnalysis;
using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class ServiceBusDeadLetterService : IDeadLetterService, IAsyncDisposable
{
    private readonly int _batchSize;
    private readonly ServiceBusSubscriptionOptions _options;
    private readonly IOptions<CdpOptions> _cdpOptions;
    private readonly IWebProxy _webProxy;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;

    public ServiceBusDeadLetterService(
        int batchSize,
        ServiceBusSubscriptionOptions options,
        IOptions<CdpOptions> cdpOptions,
        IWebProxy webProxy
    )
    {
        _batchSize = batchSize <= 0 ? 100 : batchSize;
        _options = options;
        _cdpOptions = cdpOptions;
        _webProxy = webProxy;

        _client = CreateClient();
        _receiver = _client.CreateReceiver(
            _options.Topic,
            _options.Subscription,
            new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
            }
        );
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
        await _receiver.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task<int> PeekTotalMessageCount(CancellationToken cancellationToken)
    {
        long? sequenceNumber = null;
        var total = 0;

        while (true)
        {
            var messages = await _receiver.PeekMessagesAsync(_batchSize, sequenceNumber, cancellationToken);
            total += messages.Count;

            if (messages.Count < _batchSize)
                return total;

            sequenceNumber = messages[^1].SequenceNumber + 1;

            await Task.Delay(100, cancellationToken);
        }
    }
}
