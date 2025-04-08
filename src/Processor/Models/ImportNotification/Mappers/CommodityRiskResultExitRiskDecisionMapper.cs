using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultExitRiskDecisionMapper
{
    public static IpaffsDataApi.CommodityRiskResultExitRiskDecision? Map(CommodityRiskResultExitRiskDecision? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            CommodityRiskResultExitRiskDecision.Required => IpaffsDataApi.CommodityRiskResultExitRiskDecision.Required,
            CommodityRiskResultExitRiskDecision.Notrequired => IpaffsDataApi
                .CommodityRiskResultExitRiskDecision
                .Notrequired,
            CommodityRiskResultExitRiskDecision.Inconclusive => IpaffsDataApi
                .CommodityRiskResultExitRiskDecision
                .Inconclusive,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
