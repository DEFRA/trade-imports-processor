using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Data.Entities;
using MongoDB.Driver;

namespace Defra.TradeImportsProcessor.Processor.Data.Mongo;

[ExcludeFromCodeCoverage]
public class MongoDbContext : IDbContext
{
    private readonly ILogger<MongoDbContext> _logger;

    public MongoDbContext(IMongoDatabase database, ILogger<MongoDbContext> logger)
    {
        _logger = logger;

        Database = database;
        RawMessages = new MongoCollectionSet<RawMessageEntity>(this);
    }

    internal IMongoDatabase Database { get; }
    internal MongoDbTransaction? ActiveTransaction { get; private set; }

    public IMongoCollectionSet<RawMessageEntity> RawMessages { get; }

    public async Task StartTransaction(CancellationToken cancellationToken)
    {
        var session = await Database.Client.StartSessionAsync(cancellationToken: cancellationToken);
        session.StartTransaction();

        ActiveTransaction = new MongoDbTransaction(session);
    }

    public async Task CommitTransaction(CancellationToken cancellationToken)
    {
        if (ActiveTransaction is null)
            throw new InvalidOperationException("No active transaction");

        await ActiveTransaction.Commit(cancellationToken);

        ActiveTransaction = null;
    }

    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        try
        {
            await RawMessages.Save(cancellationToken);
        }
        catch (MongoCommandException mongoCommandException) when (mongoCommandException.Code == 112)
        {
            const string message = "Mongo write conflict - consumer will retry";
            _logger.LogWarning(mongoCommandException, message);

            // WriteConflict error: this operation conflicted with another operation. Please retry your operation or multi-document transaction
            // - retries are built into consumers of the data API
            throw new ConcurrencyException(message, mongoCommandException);
        }
        catch (MongoWriteException mongoWriteException) when (mongoWriteException.WriteError.Code == 11000)
        {
            const string message = "Mongo write error - consumer will retry";
            _logger.LogWarning(mongoWriteException, message);

            // A write operation resulted in an error. WriteError: { Category : "DuplicateKey", Code : 11000 }
            // - retries are built into consumers of the data API
            throw new ConcurrencyException(message, mongoWriteException);
        }
    }
}
