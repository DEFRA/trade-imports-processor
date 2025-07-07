namespace Defra.TradeImportsProcessor.Processor.Metrics;

public interface IDeadLetterService
{
    Task<int> PeekTotalMessageCount(CancellationToken cancellationToken);
}
