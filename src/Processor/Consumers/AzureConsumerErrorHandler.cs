using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Options;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AzureConsumerErrorHandler<T>(
    IOptions<ServiceBusOptions> options,
    ILogger<AzureConsumerErrorHandler<T>> logger
) : ServiceBusConsumerErrorHandler<T>
{
    public override Task<ProcessResult> OnHandleError(
        T message,
        IConsumerContext consumerContext,
        Exception exception,
        int attempts
    )
    {
        // If we cannot deserialise, then do not wait for all attempts,
        // dead letter the message early
        if (
            exception is JsonException { Path: not null } jsonException
            && jsonException.Message.Contains("deserialization")
        )
        {
            logger.LogWarning("Dead letter message early due to JSON deserialisation exception");

            return Task.FromResult(DeadLetter());
        }

        if (attempts >= options.Value.AttemptsDeadLetterTolerance)
        {
            logger.LogWarning("Max attempts reached, dead lettering message");
        }

        return Task.FromResult(ProcessResult.Failure);
    }
}
