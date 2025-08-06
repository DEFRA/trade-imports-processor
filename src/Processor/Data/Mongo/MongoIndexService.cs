using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Data.Entities;
using MongoDB.Driver;

namespace Defra.TradeImportsProcessor.Processor.Data.Mongo;

[ExcludeFromCodeCoverage]
public class MongoIndexService(IMongoDatabase database, ILogger<MongoIndexService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateIndex(
            "UpdatedIdx",
            Builders<RawMessageEntity>.IndexKeys.Ascending(x => x.Updated),
            cancellationToken: cancellationToken
        );

        await CreateIndex(
            "UpdatesIdx",
            Builders<RawMessageEntity>
                .IndexKeys.Ascending(x => x.Updated)
                .Ascending(x => x.ResourceId)
                .Ascending(x => x.ResourceType)
                .Ascending(x => x.MessageId),
            cancellationToken: cancellationToken
        );

        await CreateTtlIndex(
            "ExpiresAtTtlIdx",
            Builders<RawMessageEntity>.IndexKeys.Ascending(x => x.ExpiresAt),
            cancellationToken: cancellationToken
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task CreateIndex<T>(
        string name,
        IndexKeysDefinition<T> keys,
        bool unique = false,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var indexModel = new CreateIndexModel<T>(
                keys,
                new CreateIndexOptions
                {
                    Name = name,
                    Background = true,
                    Unique = unique,
                }
            );
            await database
                .GetCollection<T>(typeof(T).DataEntityName())
                .Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to Create index {Name} on {Collection}", name, typeof(T).DataEntityName());
        }
    }

    private async Task CreateTtlIndex<T>(
        string name,
        IndexKeysDefinition<T> keys,
        TimeSpan? expireAfter = null,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var indexModel = new CreateIndexModel<T>(
                keys,
                new CreateIndexOptions
                {
                    Name = name,
                    Background = true,
                    ExpireAfter = expireAfter ?? TimeSpan.Zero,
                }
            );
            await database
                .GetCollection<T>(typeof(T).DataEntityName())
                .Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to Create TTL index {Name} on {Collection}", name, typeof(T).DataEntityName());
        }
    }
}
