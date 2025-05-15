using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityCheckMapper
{
    public static IpaffsDataApi.CommodityCheck Map(CommodityChecks? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.CommodityCheck
        {
            UniqueComplementId = from.UniqueComplementId,
            Checks = from.Checks?.Select(InspectionCheckMapper.Map).ToArray(),
            ValidityPeriod = 0,
        };

        return to;
    }
}
