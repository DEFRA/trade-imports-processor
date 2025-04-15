using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityChecksMapper
{
    public static IpaffsDataApi.CommodityChecks Map(CommodityChecks? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.CommodityChecks();
        to.UniqueComplementId = from?.UniqueComplementId;
        to.Checks = from?.Checks?.Select(x => InspectionCheckMapper.Map(x)).ToArray();
        to.ValidityPeriod = from?.ValidityPeriod;
        return to;
    }
}
