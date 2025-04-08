using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ChedppNotAcceptableReasonCommodityOrPackageMapper
{
    public static IpaffsDataApi.ChedppNotAcceptableReasonCommodityOrPackage? Map(
        ChedppNotAcceptableReasonCommodityOrPackage? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ChedppNotAcceptableReasonCommodityOrPackage.C => IpaffsDataApi
                .ChedppNotAcceptableReasonCommodityOrPackage
                .C,
            ChedppNotAcceptableReasonCommodityOrPackage.P => IpaffsDataApi
                .ChedppNotAcceptableReasonCommodityOrPackage
                .P,
            ChedppNotAcceptableReasonCommodityOrPackage.Cp => IpaffsDataApi
                .ChedppNotAcceptableReasonCommodityOrPackage
                .Cp,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
