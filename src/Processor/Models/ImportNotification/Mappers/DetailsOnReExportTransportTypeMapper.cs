using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DetailsOnReExportTransportTypeEnumMapper
{
    public static IpaffsDataApi.DetailsOnReExportTransportType? Map(DetailsOnReExportTransportType? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            DetailsOnReExportTransportType.Rail => IpaffsDataApi.DetailsOnReExportTransportType.Rail,
            DetailsOnReExportTransportType.Plane => IpaffsDataApi.DetailsOnReExportTransportType.Plane,
            DetailsOnReExportTransportType.Ship => IpaffsDataApi.DetailsOnReExportTransportType.Ship,
            DetailsOnReExportTransportType.Road => IpaffsDataApi.DetailsOnReExportTransportType.Road,
            DetailsOnReExportTransportType.Other => IpaffsDataApi.DetailsOnReExportTransportType.Other,
            DetailsOnReExportTransportType.CShipRoad => IpaffsDataApi.DetailsOnReExportTransportType.CShipRoad,
            DetailsOnReExportTransportType.CShipRail => IpaffsDataApi.DetailsOnReExportTransportType.CShipRail,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
