using Azure.Messaging.ServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

public static class ServiceBusReceiverExtensions
{
    public static async Task<int> PeekTotalMessageCount(
        this ServiceBusReceiver receiver,
        int batchSize = 100,
        int millisecondsDelay = 100,
        CancellationToken cancellationToken = default
    )
    {
        long? sequenceNumber = null;
        var total = 0;

        while (true)
        {
            var messages = await receiver.PeekMessagesAsync(batchSize, sequenceNumber, cancellationToken);
            total += messages.Count;

            if (messages.Count < batchSize)
                return total;

            sequenceNumber = messages[^1].SequenceNumber + 1;

            await Task.Delay(millisecondsDelay, cancellationToken);
        }
    }
}
