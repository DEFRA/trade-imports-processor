using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultMapper
{
    public static IpaffsDataApi.CommodityRiskResult Map(CommodityRiskResult? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.CommodityRiskResult
        {
            RiskDecision = from.RiskDecision,
            ExitRiskDecision = from.ExitRiskDecision,
            HmiDecision = from.HmiDecision,
            PhsiDecision = from.PhsiDecision,
            PhsiClassification = from.PhsiClassification,
            Phsi = PhsiMapper.Map(from.Phsi),
            UniqueId = from.UniqueId,
            EppoCode = from.EppoCode,
            Variety = from.Variety,
            IsWoody = from.IsWoody,
            IndoorOutdoor = from.IndoorOutdoor,
            Propagation = from.Propagation,
            PhsiRuleType = from.PhsiRuleType,
        };

        return to;
    }
}
