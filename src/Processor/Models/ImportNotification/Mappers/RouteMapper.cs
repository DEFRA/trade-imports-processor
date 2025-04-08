#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

using Route = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Route;

public static class RouteMapper
{
    public static IpaffsDataApi.Route Map(Route? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.Route();
        to.TransitingStates = from?.TransitingStates;
        return to;
    }
}
