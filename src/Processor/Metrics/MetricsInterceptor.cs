using System.Diagnostics.CodeAnalysis;
using System.Net;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class MetricsInterceptor<TMessage>(IConsumerMetrics consumerMetrics) : IConsumerInterceptor<TMessage>
    where TMessage : notnull
{
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        var startingTimestamp = TimeProvider.System.GetTimestamp();
        var consumerName = context.Consumer.GetType().Name;
        var resourceType = context.GetResourceType();

        try
        {
            consumerMetrics.Start(context.Path, consumerName, resourceType);

            return await next();
        }
        catch (HttpRequestException httpRequestException)
            when (httpRequestException.StatusCode == HttpStatusCode.Conflict)
        {
            consumerMetrics.Warn(context.Path, consumerName, resourceType, httpRequestException);

            throw;
        }
        catch (Exception exception)
        {
            consumerMetrics.Faulted(context.Path, consumerName, resourceType, exception);

            throw;
        }
        finally
        {
            consumerMetrics.Complete(
                context.Path,
                consumerName,
                TimeProvider.System.GetElapsedTime(startingTimestamp).TotalMilliseconds,
                resourceType
            );
        }
    }
}
