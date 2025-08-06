namespace Defra.TradeImportsProcessor.Processor.Data;

public interface IDbTransaction : IDisposable
{
    Task Commit(CancellationToken cancellationToken);
}
