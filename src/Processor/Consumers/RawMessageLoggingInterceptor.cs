using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Data;
using Defra.TradeImportsProcessor.Processor.Data.Entities;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using SlimMessageBus;
using SlimMessageBus.Host.Interceptor;
using Gmr = Defra.TradeImportsProcessor.Processor.Models.Gmrs.Gmr;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

[ExcludeFromCodeCoverage]
public class RawMessageLoggingInterceptor<TMessage>(
    IDbContext dbContext,
    ILogger<RawMessageLoggingInterceptor<TMessage>> logger,
    IOptions<RawMessageLoggingOptions> options
) : IConsumerInterceptor<TMessage>
{
    public async Task<object> OnHandle(TMessage message, Func<Task<object>> next, IConsumerContext context)
    {
        try
        {
            if (message is not JsonElement jsonElement)
                return await next();

            var resourceType = context.GetResourceType();
            if (resourceType == ResourceTypes.Unknown)
                return await next();

            await LogRawMessage(context, resourceType, jsonElement);

            return await next();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
#pragma warning disable S2139
        catch (Exception exception)
#pragma warning restore S2139
        {
            logger.LogWarning(exception, "Failed to log raw message in {Method}", nameof(OnHandle));

            throw;
        }
    }

    private async Task LogRawMessage(IConsumerContext context, string resourceType, JsonElement jsonElement)
    {
        try
        {
            var entity = new RawMessageEntity
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ResourceId = GetResourceId(resourceType, jsonElement, context),
                ResourceType = resourceType,
                MessageId = context.GetMessageId(),
                Headers = context.Headers.ToDictionary(x => x.Key, x => x.Value?.ToString()),
                Message = jsonElement.GetRawText(),
                ExpiresAt = DateTime.UtcNow.AddDays(options.Value.TtlDays),
            };

            await dbContext.StartTransaction(context.CancellationToken);
            dbContext.RawMessages.Insert(entity);
            await dbContext.SaveChanges(context.CancellationToken);
            await dbContext.CommitTransaction(context.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Failed to log raw message in {Method}", nameof(LogRawMessage));

            // Intentionally swallowed
        }
    }

    private static string GetResourceId(string resourceType, JsonElement jsonElement, IConsumerContext context)
    {
        var result = resourceType switch
        {
            ResourceTypes.Gmr => jsonElement.GetProperty(nameof(Gmr.GmrId)).GetString(),
            ResourceTypes.ImportPreNotification => jsonElement
                .GetProperty(
                    // Currently case-sensitive based on JsonPropertyName on type
                    char.ToLower(nameof(ImportNotification.ReferenceNumber)[0])
                        + nameof(ImportNotification.ReferenceNumber)[1..]
                )
                .GetString(),
            _ => context.GetResourceId(),
        };

        return result ?? "Unknown";
    }
}
