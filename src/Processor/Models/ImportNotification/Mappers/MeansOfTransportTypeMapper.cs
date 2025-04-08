#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class MeansOfTransportTypeEnumMapper
{
    public static IpaffsDataApi.MeansOfTransportType? Map(MeansOfTransportType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            MeansOfTransportType.Aeroplane => IpaffsDataApi.MeansOfTransportType.Aeroplane,
            MeansOfTransportType.RoadVehicle => IpaffsDataApi.MeansOfTransportType.RoadVehicle,
            MeansOfTransportType.RailwayWagon => IpaffsDataApi.MeansOfTransportType.RailwayWagon,
            MeansOfTransportType.Ship => IpaffsDataApi.MeansOfTransportType.Ship,
            MeansOfTransportType.Other => IpaffsDataApi.MeansOfTransportType.Other,
            MeansOfTransportType.RoadVehicleAeroplane => IpaffsDataApi.MeansOfTransportType.RoadVehicleAeroplane,
            MeansOfTransportType.ShipRailwayWagon => IpaffsDataApi.MeansOfTransportType.ShipRailwayWagon,
            MeansOfTransportType.ShipRoadVehicle => IpaffsDataApi.MeansOfTransportType.ShipRoadVehicle,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
