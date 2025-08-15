using System.Diagnostics.CodeAnalysis;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class PublisherMetricsInterceptor<TMessage>(
    IPublisherMetrics publisherMetrics,
    ILogger<PublisherMetricsInterceptor<TMessage>> logger
) : IPublishInterceptor<TMessage>
    where TMessage : notnull
{
    [SuppressMessage(
        "Minor Code Smell",
        "S6667:Logging in a catch clause should pass the caught exception as a parameter. "
            + "- the exception is thrown, we do not want to log here as the logging interceptor will do that"
    )]
    public async Task OnHandle(TMessage message, Func<Task> next, IProducerContext context)
    {
        var startingTimestamp = TimeProvider.System.GetTimestamp();
        var publisherType = context.Headers.FirstOrDefault(x => x.Key == "PublisherType").Value as string ?? "Unknown";
        var resourceType = context.Headers.FirstOrDefault(x => x.Key == "MessageType").Value is string messageType
            ? Type.GetType(messageType)?.Name ?? "Unknown"
            : "Unknown";

        logger.LogInformation("{Path}", context.Path);
        logger.LogInformation("{BusType}", context.Bus.GetType().FullName);
        logger.LogInformation(
            "{Headers}",
            string.Join(", ", context.Headers.Select(kvp => $"{kvp.Key}: {kvp.Value} {kvp.Value.GetType().FullName}"))
        );
        logger.LogInformation(
            "{Properties}",
            string.Join(
                ", ",
                context.Properties.Select(kvp => $"{kvp.Key}: {kvp.Value} {kvp.Value.GetType().FullName}")
            )
        );

        try
        {
            publisherMetrics.Start(context.Path, publisherType, resourceType);

            await next();
        }
        catch (Exception exception)
        {
            publisherMetrics.Faulted(context.Path, publisherType, resourceType, exception);

            LogForTriaging("Faulted", resourceType);

            throw;
        }
        finally
        {
            publisherMetrics.Complete(
                context.Path,
                publisherType,
                resourceType,
                TimeProvider.System.GetElapsedTime(startingTimestamp).TotalMilliseconds
            );
        }
    }

    /// <summary>
    /// Intentionally an information log as this supports triaging, not alerting.
    /// The logging interceptor will log for the benefit of alerting.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="publishingType"></param>
    private void LogForTriaging(string level, string publishingType)
    {
        logger.LogInformation("{Level} publishing type {Publish}", level, publishingType);
    }
}
