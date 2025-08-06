using Defra.TradeImportsProcessor.Processor.Data.Entities;

namespace Defra.TradeImportsProcessor.Processor.Data;

public interface IDbContext
{
    IMongoCollectionSet<RawMessageEntity> RawMessages { get; }

    Task SaveChanges(CancellationToken cancellationToken);

    Task StartTransaction(CancellationToken cancellationToken);

    Task CommitTransaction(CancellationToken cancellationToken);
}
