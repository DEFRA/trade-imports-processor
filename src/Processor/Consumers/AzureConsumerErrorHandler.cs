using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Microsoft.Extensions.Options;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AzureConsumerErrorHandler<T>(IConsumerMetrics consumerMetrics, IOptions<ServiceBusOptions> options)
    : ServiceBusConsumerErrorHandler<T>
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

        if (attempts >= options.Value.AttemptsDeadLetterTolerance)
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
