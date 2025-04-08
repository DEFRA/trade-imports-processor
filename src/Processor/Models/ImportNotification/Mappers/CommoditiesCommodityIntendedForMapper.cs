using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommoditiesCommodityIntendedForMapper
{
    public static IpaffsDataApi.CommoditiesCommodityIntendedFor? Map(CommoditiesCommodityIntendedFor? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            CommoditiesCommodityIntendedFor.Human => IpaffsDataApi.CommoditiesCommodityIntendedFor.Human,
            CommoditiesCommodityIntendedFor.Feedingstuff => IpaffsDataApi.CommoditiesCommodityIntendedFor.Feedingstuff,
            CommoditiesCommodityIntendedFor.Further => IpaffsDataApi.CommoditiesCommodityIntendedFor.Further,
            CommoditiesCommodityIntendedFor.Other => IpaffsDataApi.CommoditiesCommodityIntendedFor.Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
