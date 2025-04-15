using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultMapper
{
    public static IpaffsDataApi.CommodityRiskResult Map(CommodityRiskResult? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.CommodityRiskResult();
        to.RiskDecision = CommodityRiskResultRiskDecisionEnumMapper.Map(from?.RiskDecision);
        to.ExitRiskDecision = CommodityRiskResultExitRiskDecisionMapper.Map(from?.ExitRiskDecision);
        to.HmiDecision = CommodityRiskResultHmiDecisionMapper.Map(from?.HmiDecision);
        to.PhsiDecision = CommodityRiskResultPhsiDecisionEnumMapper.Map(from?.PhsiDecision);
        to.PhsiClassification = CommodityRiskResultPhsiClassificationEnumMapper.Map(from?.PhsiClassification);
        to.Phsi = PhsiMapper.Map(from?.Phsi);
        to.UniqueId = from?.UniqueId;
        to.EppoCode = from?.EppoCode;
        to.Variety = from?.Variety;
        to.IsWoody = from?.IsWoody;
        to.IndoorOutdoor = from?.IndoorOutdoor;
        to.Propagation = from?.Propagation;
        to.PhsiRuleType = from?.PhsiRuleType;
        return to;
    }
}
