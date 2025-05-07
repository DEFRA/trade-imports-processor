using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

using Route = Route;

public static class RouteMapper
{
    public static IpaffsDataApi.Route Map(Route? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Route { TransitingStates = from.TransitingStates };

        return to;
    }
}
