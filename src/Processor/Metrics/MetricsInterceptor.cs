using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class MetricsInterceptor<TMessage>(ConsumerMetrics consumerMetrics) : IConsumerInterceptor<TMessage>
    where TMessage : notnull
{
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        var startingTimestamp = TimeProvider.System.GetTimestamp();
        var resourceType = context.GetResourceType();

        try
        {
            consumerMetrics.Start(context.Path, context.Consumer.GetType().Name, resourceType);
            return await next();
        }
        catch (Exception exception)
        {
            consumerMetrics.Faulted(context.Path, context.Consumer.GetType().Name, resourceType, exception);
            throw;
        }
        finally
        {
            consumerMetrics.Complete(
                context.Path,
                context.Consumer.GetType().Name,
                TimeProvider.System.GetElapsedTime(startingTimestamp).TotalMilliseconds,
                resourceType
            );
        }
    }
}
