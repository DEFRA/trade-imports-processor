using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Metrics;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AmazonSQS;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AwsConsumerErrorHandler<T>(IConsumerMetrics consumerMetrics) : SqsConsumerErrorHandler<T>
{
    public override Task<ProcessResult> OnHandleError(
        T message,
        IConsumerContext consumerContext,
        Exception exception,
        int attempts
    )
    {
        // Our CDP config allows 3 attempts
        if (attempts >= 3)
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
