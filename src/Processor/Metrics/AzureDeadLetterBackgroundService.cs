using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

public class AzureDeadLetterBackgroundService : BackgroundService
{
    private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);
    private readonly ServiceBusSubscriptionOptions _options;
    private readonly string _consumerName;
    private readonly ILogger<AzureDeadLetterBackgroundService> _logger;
    private readonly IOptions<CdpOptions> _cdpOptions;
    private readonly IWebProxy _webProxy;
    private readonly AzureMetrics _azureMetrics;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;

    public AzureDeadLetterBackgroundService(
        ServiceBusSubscriptionOptions options,
        string consumerName,
        ILogger<AzureDeadLetterBackgroundService> logger,
        IOptions<CdpOptions> cdpOptions,
        IWebProxy webProxy,
        AzureMetrics azureMetrics
    )
    {
        _options = options;
        _consumerName = consumerName;
        _logger = logger;
        _cdpOptions = cdpOptions;
        _webProxy = webProxy;
        _azureMetrics = azureMetrics;

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
        // Wait for a random amount of time to minimise clashes as this will
        // run in all deployed instances of the processor
        var wait = TimeSpan.FromSeconds(Random.Shared.Next(30, 120));
        await Task.Delay(wait, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var count = await PeekTotalMessageCount(stoppingToken);

                _logger.LogInformation("Dead letter monitor for {Topic}, count {Count}", _options.Topic, count);

                _azureMetrics.DeadLetter(_consumerName, count);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get dead letter count for {Topic}", _options.Topic);
            }

            await Task.Delay(_delay, stoppingToken);
        }
    }

    private async Task<int> PeekTotalMessageCount(CancellationToken stoppingToken)
    {
        var count = 0;
        int read;
        long? from = 0;
        const int batch = 100;

        do
        {
            var messages = await _receiver.PeekMessagesAsync(batch, from, stoppingToken);

            read = messages.Count;
            count += read;

            await Task.Delay(100, stoppingToken);
        } while (read == batch);

        return count;
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
