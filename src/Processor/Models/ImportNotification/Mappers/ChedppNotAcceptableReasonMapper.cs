using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ChedppNotAcceptableReasonMapper
{
    public static IpaffsDataApi.ChedppNotAcceptableReason Map(ChedppNotAcceptableReason? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ChedppNotAcceptableReason
        {
            Reason = from.Reason,
            CommodityOrPackage = from.CommodityOrPackage,
        };

        return to;
    }
}
