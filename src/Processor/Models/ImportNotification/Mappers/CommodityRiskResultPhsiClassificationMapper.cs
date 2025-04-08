using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityRiskResultPhsiClassificationEnumMapper
{
    public static IpaffsDataApi.CommodityRiskResultPhsiClassification? Map(CommodityRiskResultPhsiClassification? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            CommodityRiskResultPhsiClassification.Mandatory => IpaffsDataApi
                .CommodityRiskResultPhsiClassification
                .Mandatory,
            CommodityRiskResultPhsiClassification.Reduced => IpaffsDataApi
                .CommodityRiskResultPhsiClassification
                .Reduced,
            CommodityRiskResultPhsiClassification.Controlled => IpaffsDataApi
                .CommodityRiskResultPhsiClassification
                .Controlled,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
