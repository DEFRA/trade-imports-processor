using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

public class AzureDeadLetterBackgroundService : BackgroundService
{
    private readonly TimeSpan _startDelay = TimeSpan.FromSeconds(60);
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(30);
    private readonly ServiceBusSubscriptionOptions _options;
    private readonly ILogger<AzureDeadLetterBackgroundService> _logger;
    private readonly IOptions<CdpOptions> _cdpOptions;
    private readonly IWebProxy _webProxy;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;

    public AzureDeadLetterBackgroundService(
        ServiceBusSubscriptionOptions options,
        ILogger<AzureDeadLetterBackgroundService> logger,
        IOptions<CdpOptions> cdpOptions,
        IWebProxy webProxy
    )
    {
        _options = options;
        _logger = logger;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(_startDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var count = 0;
                int messagesRead;
                long? lastSequenceNumber = 0;

                do
                {
                    var messages = await _receiver.PeekMessagesAsync(
                        maxMessages: 100,
                        fromSequenceNumber: lastSequenceNumber,
                        cancellationToken: stoppingToken
                    );

                    messagesRead = messages.Count;
                    count += messagesRead;
                } while (messagesRead == 100);

                _logger.LogInformation("Dead letter monitor for {Topic}, count {Count}", _options.Topic, count);

                // Would set our metric here with the DLQ count we have found

                // Considerations:
                // All deployed instances would be executing this code so we should consider a random start/jitter.
                // The volume of messages on the DLQ would impact the performance of the code above.
                // There is an "admin" client but it only works over HTTP and we would need specific permissions
                //   to call it beyond our current SAS token.
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get dead letter count for {Topic}", _options.Topic);
            }

            await Task.Delay(_delay, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _receiver.DisposeAsync();
        await _client.DisposeAsync();

        await base.StopAsync(cancellationToken);
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
}
