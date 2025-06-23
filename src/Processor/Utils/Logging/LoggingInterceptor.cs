using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;

namespace Defra.TradeImportsProcessor.Processor.Utils.Logging;

[ExcludeFromCodeCoverage]
[SuppressMessage("Major Code Smell", "S2139:Exceptions should be either logged or rethrown but not both")]
public class LoggingInterceptor<TMessage>(ILogger<LoggingInterceptor<TMessage>> logger) : IConsumerInterceptor<TMessage>
{
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        var messageId = context.GetMessageId();
        var resourceId = context.GetResourceId();
        logger.LogInformation("Processing message {MessageId} for resource {ResourceId}", messageId, resourceId);

        try
        {
            return await next();
        }
        catch (HttpRequestException httpRequestException)
            when (httpRequestException.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogWarning(
                httpRequestException,
                "409 Conflict processing message {MessageId} for resource {ResourceId}",
                messageId,
                resourceId
            );

            throw;
        }
        catch (JsonException jsonException)
        {
            logger.LogError(
                jsonException,
                "Error processing message {MessageId} for resource {ResourceId} with message: {Message}",
                messageId,
                resourceId,
                context.GetMessageBody()
            );
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Error processing message {MessageId} for resource {ResourceId}",
                messageId,
                resourceId
            );

            throw;
        }
    }
}
