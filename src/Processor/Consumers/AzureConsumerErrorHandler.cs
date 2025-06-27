using System.Text.Json;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AzureConsumerErrorHandler<T>(ILogger<AzureConsumerErrorHandler<T>> logger)
    : ServiceBusConsumerErrorHandler<T>
{
    public override Task<ProcessResult> OnHandleError(
        T message,
        IConsumerContext consumerContext,
        Exception exception,
        int attempts
    )
    {
        if (exception is JsonException jsonException)
        {
            logger.LogWarning(jsonException, "Dead letter message early due to JSON deserialisation exception");

            return Task.FromResult(DeadLetter());
        }

        return Task.FromResult(ProcessResult.Failure);
    }
}
