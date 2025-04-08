#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultRiskDecisionEnumMapper
{
    public static IpaffsDataApi.CommodityRiskResultRiskDecision? Map(CommodityRiskResultRiskDecision? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            CommodityRiskResultRiskDecision.Required => IpaffsDataApi.CommodityRiskResultRiskDecision.Required,
            CommodityRiskResultRiskDecision.Notrequired => IpaffsDataApi.CommodityRiskResultRiskDecision.Notrequired,
            CommodityRiskResultRiskDecision.Inconclusive => IpaffsDataApi.CommodityRiskResultRiskDecision.Inconclusive,
            CommodityRiskResultRiskDecision.ReenforcedCheck => IpaffsDataApi
                .CommodityRiskResultRiskDecision
                .ReenforcedCheck,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
