using System.Diagnostics.CodeAnalysis;
using System.Net;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class ConsumerMetricsInterceptor<TMessage>(
    IConsumerMetrics consumerMetrics,
    ILogger<ConsumerMetricsInterceptor<TMessage>> logger
) : IConsumerInterceptor<TMessage>
    where TMessage : notnull
{
    [SuppressMessage(
        "Minor Code Smell",
        "S6667:Logging in a catch clause should pass the caught exception as a parameter. "
            + "- the exception is thrown, we do not want to log here as the logging interceptor will do that"
    )]
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        var startingTimestamp = TimeProvider.System.GetTimestamp();
        var consumerName = context.Consumer.GetType().Name;
        var resourceType = context.GetResourceType();
        var resourceId = context.GetResourceId();

        try
        {
            consumerMetrics.Start(context.Path, consumerName, resourceType);

            return await next();
        }
        catch (HttpRequestException httpRequestException)
            when (httpRequestException.StatusCode == HttpStatusCode.Conflict)
        {
            consumerMetrics.Warn(context.Path, consumerName, resourceType, httpRequestException);

            LogForTriaging("Warn", consumerName, resourceId, resourceType);

            throw;
        }
        catch (Exception exception)
        {
            consumerMetrics.Faulted(context.Path, consumerName, resourceType, exception);

            LogForTriaging("Faulted", consumerName, resourceId, resourceType);

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

    /// <summary>
    /// Intentionally an information log as this supports triaging, not alerting.
    /// The logging interceptor will log for the benefit of alerting.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="consumerName"></param>
    /// <param name="resourceId"></param>
    /// <param name="resourceType"></param>
    private void LogForTriaging(string level, string consumerName, string resourceId, string resourceType)
    {
        logger.LogInformation(
            "{Level} consumer {Consumer} for resource {Resource} of type {Type}",
            level,
            consumerName,
            resourceId,
            resourceType
        );
    }
}
