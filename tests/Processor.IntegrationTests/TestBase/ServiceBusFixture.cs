using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

/// <summary>
/// Prevents creation of too many connections to the Service Bus Emulator and going over the Connections Quota
/// </summary>
public class ServiceBusFixture : IAsyncLifetime
{
    private const string ConnectionString =
        "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true";

    private readonly Dictionary<string, ServiceBusFixtureClient> _clients = new();

    private ServiceBusClient? _serviceBusClient;

    public Task InitializeAsync()
    {
        _serviceBusClient = new ServiceBusClient(ConnectionString);

        return Task.CompletedTask;
    }

    public async Task PurgeSubscription(string topicName, string subscriptionName)
    {
        if (_serviceBusClient is null)
            throw new InvalidOperationException("Service Bus client not initialized");

        await using var receiver = _serviceBusClient.CreateReceiver(
            topicName,
            subscriptionName,
            new ServiceBusReceiverOptions { ReceiveMode = ServiceBusReceiveMode.PeekLock }
        );

        var timeout = TimeSpan.FromSeconds(30);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            var messages = await receiver.ReceiveMessagesAsync(100, TimeSpan.FromSeconds(1));

            if (messages.Count == 0)
                return;

            foreach (var message in messages)
            {
                await receiver.CompleteMessageAsync(message);
            }
        }

        throw new TimeoutException(
            $"Failed to purge Service Bus subscription '{topicName}/{subscriptionName}' within {timeout}."
        );
    }

    public ServiceBusFixtureClient GetClient(string topicName, string subscriptionName)
    {
        var clientKey = $"{topicName}:{subscriptionName}";

        if (!_clients.TryGetValue(clientKey, out var client))
        {
            client = new ServiceBusFixtureClient(topicName, subscriptionName);
            _clients.Add(clientKey, client);
        }

        return client;
    }

    public async Task DisposeAsync()
    {
        foreach (var client in _clients.Values)
        {
            await client.DisposeAsync();
        }

        await Dispose(_serviceBusClient);
    }

    private static async Task Dispose(ServiceBusClient? disposable)
    {
        if (disposable != null)
            await disposable.DisposeAsync();
    }
}
