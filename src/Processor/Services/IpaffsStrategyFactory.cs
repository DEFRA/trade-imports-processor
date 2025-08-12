namespace Defra.TradeImportsProcessor.Processor.Services;

public interface IIpaffsStrategyFactory
{
    bool TryGetIpaffsStrategy(string? subResourceType, out IIpaffsStrategy? strategy);
}

public class IpaffsStrategyFactory(IEnumerable<IIpaffsStrategy> strategies) : IIpaffsStrategyFactory
{
    public bool TryGetIpaffsStrategy(string? subResourceType, out IIpaffsStrategy? strategy)
    {
        strategy = strategies.FirstOrDefault(strategy => strategy.SupportedSubResourceType == subResourceType);

        return strategy != null;
    }
}
