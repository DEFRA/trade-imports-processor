using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultPhsiDecisionEnumMapper
{
    public static IpaffsDataApi.CommodityRiskResultPhsiDecision? Map(CommodityRiskResultPhsiDecision? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            CommodityRiskResultPhsiDecision.Required => IpaffsDataApi.CommodityRiskResultPhsiDecision.Required,
            CommodityRiskResultPhsiDecision.Notrequired => IpaffsDataApi.CommodityRiskResultPhsiDecision.Notrequired,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
