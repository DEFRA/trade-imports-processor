using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ChedppNotAcceptableReasonMapper
{
    public static IpaffsDataApi.ChedppNotAcceptableReason Map(ChedppNotAcceptableReason? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ChedppNotAcceptableReason();
        to.Reason = ChedppNotAcceptableReasonReasonMapper.Map(from.Reason);
        to.CommodityOrPackage = ChedppNotAcceptableReasonCommodityOrPackageMapper.Map(from.CommodityOrPackage);
        return to;
    }
}
