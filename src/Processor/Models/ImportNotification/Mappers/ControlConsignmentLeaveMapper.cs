using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlConsignmentLeaveEnumMapper
{
    public static IpaffsDataApi.ControlConsignmentLeave? Map(ControlConsignmentLeave? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            ControlConsignmentLeave.Yes => IpaffsDataApi.ControlConsignmentLeave.Yes,
            ControlConsignmentLeave.No => IpaffsDataApi.ControlConsignmentLeave.No,
            ControlConsignmentLeave.ItHasBeenDestroyed => IpaffsDataApi.ControlConsignmentLeave.ItHasBeenDestroyed,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
