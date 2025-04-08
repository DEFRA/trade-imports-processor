using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultHmiDecisionMapper
{
    public static IpaffsDataApi.CommodityRiskResultHmiDecision? Map(CommodityRiskResultHmiDecision? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            CommodityRiskResultHmiDecision.Required => IpaffsDataApi.CommodityRiskResultHmiDecision.Required,
            CommodityRiskResultHmiDecision.Notrequired => IpaffsDataApi.CommodityRiskResultHmiDecision.Notrequired,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
