using System.Linq.Expressions;
using Defra.TradeImportsProcessor.Processor.Data.Entities;
using MongoDB.Driver;

namespace Defra.TradeImportsProcessor.Processor.Data;

public interface IMongoCollectionSet<T> : IQueryable<T>
    where T : IDataEntity
{
    IMongoCollection<T> Collection { get; }

    Task<T?> Find(string id, CancellationToken cancellationToken);

    Task<List<T>> FindMany(Expression<Func<T, bool>> query, CancellationToken cancellationToken);

    void Insert(T item);

    void Update(T item, string etag);

    void Update(T item, Action<IFieldUpdateBuilder<T>> patch, string etag);

    Task Save(CancellationToken cancellationToken);
}
