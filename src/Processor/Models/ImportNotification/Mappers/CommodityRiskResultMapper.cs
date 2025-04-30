using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultMapper
{
    public static IpaffsDataApi.CommodityRiskResult Map(CommodityRiskResult? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.CommodityRiskResult();
        to.RiskDecision = from.RiskDecision;
        to.ExitRiskDecision = from.ExitRiskDecision;
        to.HmiDecision = from.HmiDecision;
        to.PhsiDecision = from.PhsiDecision;
        to.PhsiClassification = from.PhsiClassification;
        to.Phsi = PhsiMapper.Map(from.Phsi);
        to.UniqueId = from.UniqueId;
        to.EppoCode = from.EppoCode;
        to.Variety = from.Variety;
        to.IsWoody = from.IsWoody;
        to.IndoorOutdoor = from.IndoorOutdoor;
        to.Propagation = from.Propagation;
        to.PhsiRuleType = from.PhsiRuleType;
        return to;
    }
}
