using Azure.Messaging.ServiceBus;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

public class ServiceBusFixture : TestBase, IAsyncLifetime
{
    private const string ConnectionString =
        "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true";

    private readonly Dictionary<string, ServiceBusReceiver> _receivers = new();
    private readonly Dictionary<string, ServiceBusReceiver> _deadLetterReceivers = new();
    private readonly Dictionary<string, ServiceBusSender> _senders = new();

    private ServiceBusClient? _serviceBusClient;
    private ServiceBusReceiver? _serviceBusReceiver;
    private ServiceBusReceiver? _serviceBusDeadLetterReceiver;
    private ServiceBusSender? _serviceBusSender;

    public ServiceBusReceiver Receiver =>
        _serviceBusReceiver ?? throw new InvalidOperationException("Service Bus Receiver not initialized");

    public ServiceBusReceiver DeadLetterReceiver =>
        _serviceBusDeadLetterReceiver
        ?? throw new InvalidOperationException("Service Bus Dead Letter Receiver not initialized");

    public ServiceBusSender Sender =>
        _serviceBusSender ?? throw new InvalidOperationException("Service Bus Sender not initialized");

    public Task InitializeAsync()
    {
        _serviceBusClient = new ServiceBusClient(ConnectionString);

        return Task.CompletedTask;
    }

    public void Using(string topicName, string subscriptionName)
    {
        if (!_receivers.TryGetValue(topicName, out _serviceBusReceiver))
        {
            _serviceBusReceiver = _serviceBusClient!.CreateReceiver(topicName, subscriptionName);
            _receivers.Add(topicName, _serviceBusReceiver);
        }

        if (!_deadLetterReceivers.TryGetValue(topicName, out _serviceBusDeadLetterReceiver))
        {
            _serviceBusDeadLetterReceiver = _serviceBusClient!.CreateReceiver(
                topicName,
                subscriptionName,
                new ServiceBusReceiverOptions
                {
                    SubQueue = SubQueue.DeadLetter,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock,
                }
            );
            _deadLetterReceivers.Add(topicName, _serviceBusDeadLetterReceiver);
        }

        if (!_senders.TryGetValue(topicName, out _serviceBusSender))
        {
            _serviceBusSender = _serviceBusClient!.CreateSender(topicName);
            _senders.Add(topicName, _serviceBusSender);
        }
    }

    public async Task DisposeAsync()
    {
        // Need to clear out the topic
        foreach (var serviceBusReceiver in _receivers.Values)
        {
            await Dispose(serviceBusReceiver);
        }
        foreach (var serviceBusReceiver in _deadLetterReceivers.Values)
        {
            await Dispose(serviceBusReceiver);
        }
        foreach (var serviceBusSender in _senders.Values)
        {
            await Dispose(serviceBusSender);
        }
        await Dispose(_serviceBusClient);
    }

    private static async Task Dispose(IAsyncDisposable? disposable)
    {
        if (disposable != null)
            await disposable.DisposeAsync();
    }
}

[CollectionDefinition("UsesServiceBus")]
public class ServiceBusCollection : ICollectionFixture<ServiceBusFixture> { }
