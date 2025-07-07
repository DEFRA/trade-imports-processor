using Azure.Messaging.ServiceBus;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

public class ServiceBusTestBase(string topicName, string subscriptionName) : IAsyncLifetime
{
    private const string ConnectionString =
        "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true";

    private ServiceBusClient? _serviceBusClient;
    private ServiceBusReceiver? _serviceBusReceiver;
    private ServiceBusReceiver? _serviceBusDeadLetterReceiver;
    private ServiceBusSender? _serviceBusSender;

    protected ServiceBusReceiver Receiver =>
        _serviceBusReceiver ?? throw new InvalidOperationException("Service Bus Receiver not initialized");

    protected ServiceBusReceiver DeadLetterReceiver =>
        _serviceBusDeadLetterReceiver
        ?? throw new InvalidOperationException("Service Bus Dead Letter Receiver not initialized");

    protected ServiceBusSender Sender =>
        _serviceBusSender ?? throw new InvalidOperationException("Service Bus Sender not initialized");

    public Task InitializeAsync()
    {
        _serviceBusClient = new ServiceBusClient(ConnectionString);
        _serviceBusReceiver = _serviceBusClient.CreateReceiver(topicName, subscriptionName);
        _serviceBusDeadLetterReceiver = _serviceBusClient.CreateReceiver(
            topicName,
            subscriptionName,
            new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
            }
        );
        _serviceBusSender = _serviceBusClient.CreateSender(topicName);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Need to clear out the topic
        await Dispose(_serviceBusReceiver);
        await Dispose(_serviceBusDeadLetterReceiver);
        await Dispose(_serviceBusSender);
        await Dispose(_serviceBusClient);
    }

    private static async Task Dispose(IAsyncDisposable? disposable)
    {
        if (disposable != null)
            await disposable.DisposeAsync();
    }
}
