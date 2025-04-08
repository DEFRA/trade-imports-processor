using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ChedppNotAcceptableReasonReasonMapper
{
    public static IpaffsDataApi.ChedppNotAcceptableReasonReason? Map(ChedppNotAcceptableReasonReason? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ChedppNotAcceptableReasonReason.DocPhmdm => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPhmdm,
            ChedppNotAcceptableReasonReason.DocPhmdii => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPhmdii,
            ChedppNotAcceptableReasonReason.DocPa => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPa,
            ChedppNotAcceptableReasonReason.DocPic => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPic,
            ChedppNotAcceptableReasonReason.DocPill => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPill,
            ChedppNotAcceptableReasonReason.DocPed => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPed,
            ChedppNotAcceptableReasonReason.DocPmod => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPmod,
            ChedppNotAcceptableReasonReason.DocPfi => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPfi,
            ChedppNotAcceptableReasonReason.DocPnol => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPnol,
            ChedppNotAcceptableReasonReason.DocPcne => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPcne,
            ChedppNotAcceptableReasonReason.DocPadm => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPadm,
            ChedppNotAcceptableReasonReason.DocPadi => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPadi,
            ChedppNotAcceptableReasonReason.DocPpni => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPpni,
            ChedppNotAcceptableReasonReason.DocPf => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPf,
            ChedppNotAcceptableReasonReason.DocPo => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocPo,
            ChedppNotAcceptableReasonReason.DocNcevd => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocNcevd,
            ChedppNotAcceptableReasonReason.DocNcpqefi => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocNcpqefi,
            ChedppNotAcceptableReasonReason.DocNcpqebec => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocNcpqebec,
            ChedppNotAcceptableReasonReason.DocNcts => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocNcts,
            ChedppNotAcceptableReasonReason.DocNco => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocNco,
            ChedppNotAcceptableReasonReason.DocOrii => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocOrii,
            ChedppNotAcceptableReasonReason.DocOrsr => IpaffsDataApi.ChedppNotAcceptableReasonReason.DocOrsr,
            ChedppNotAcceptableReasonReason.OriOrrnu => IpaffsDataApi.ChedppNotAcceptableReasonReason.OriOrrnu,
            ChedppNotAcceptableReasonReason.PhyOrpp => IpaffsDataApi.ChedppNotAcceptableReasonReason.PhyOrpp,
            ChedppNotAcceptableReasonReason.PhyOrho => IpaffsDataApi.ChedppNotAcceptableReasonReason.PhyOrho,
            ChedppNotAcceptableReasonReason.PhyIs => IpaffsDataApi.ChedppNotAcceptableReasonReason.PhyIs,
            ChedppNotAcceptableReasonReason.PhyOrsr => IpaffsDataApi.ChedppNotAcceptableReasonReason.PhyOrsr,
            ChedppNotAcceptableReasonReason.OthCnl => IpaffsDataApi.ChedppNotAcceptableReasonReason.OthCnl,
            ChedppNotAcceptableReasonReason.OthO => IpaffsDataApi.ChedppNotAcceptableReasonReason.OthO,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
