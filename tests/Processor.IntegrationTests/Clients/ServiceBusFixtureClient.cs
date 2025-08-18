using Azure.Messaging.ServiceBus;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;

public class ServiceBusFixtureClient : IAsyncDisposable
{
    private const string ConnectionString =
        "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true";

    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusReceiver? _serviceBusReceiver;
    private readonly ServiceBusReceiver? _serviceBusDeadLetterReceiver;
    private readonly ServiceBusSender? _serviceBusSender;

    public ServiceBusReceiver Receiver =>
        _serviceBusReceiver ?? throw new InvalidOperationException("Service Bus Receiver not initialized");

    public ServiceBusReceiver DeadLetterReceiver =>
        _serviceBusDeadLetterReceiver
        ?? throw new InvalidOperationException("Service Bus Dead Letter Receiver not initialized");

    public ServiceBusSender Sender =>
        _serviceBusSender ?? throw new InvalidOperationException("Service Bus Sender not initialized");

    public ServiceBusFixtureClient(string topicName, string subscriptionName)
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
    }

    public async ValueTask DisposeAsync()
    {
        await Dispose(_serviceBusReceiver);
        await Dispose(_serviceBusDeadLetterReceiver);
        await Dispose(_serviceBusSender);
        await Dispose(_serviceBusClient);
        GC.SuppressFinalize(this);
    }

    private static async Task Dispose(IAsyncDisposable? disposable)
    {
        if (disposable != null)
            await disposable.DisposeAsync();
    }
}
