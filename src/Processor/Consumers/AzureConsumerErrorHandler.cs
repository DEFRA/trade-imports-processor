using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Metrics;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AzureConsumerErrorHandler<T>(IConsumerMetrics consumerMetrics) : ServiceBusConsumerErrorHandler<T>
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
            DeadLetterMetric(consumerContext, jsonException);

            return Task.FromResult(DeadLetter());
        }

        // Our Azure subscription allow 10 attempts and it's not
        // something that can be controlled in our connection string
        if (attempts >= 10)
        {
            DeadLetterMetric(consumerContext, exception);
        }

        return Task.FromResult(ProcessResult.Failure);
    }

    private void DeadLetterMetric(IConsumerContext consumerContext, Exception exception)
    {
        consumerMetrics.DeadLetter(
            consumerContext.Path,
            consumerContext.Consumer.GetType().Name,
            consumerContext.GetResourceType(),
            exception
        );
    }
}
