using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ChedppNotAcceptableReasonReason
{
    [EnumMember(Value = "doc-phmdm")]
    DocPhmdm,

    [EnumMember(Value = "doc-phmdii")]
    DocPhmdii,

    [EnumMember(Value = "doc-pa")]
    DocPa,

    [EnumMember(Value = "doc-pic")]
    DocPic,

    [EnumMember(Value = "doc-pill")]
    DocPill,

    [EnumMember(Value = "doc-ped")]
    DocPed,

    [EnumMember(Value = "doc-pmod")]
    DocPmod,

    [EnumMember(Value = "doc-pfi")]
    DocPfi,

    [EnumMember(Value = "doc-pnol")]
    DocPnol,

    [EnumMember(Value = "doc-pcne")]
    DocPcne,

    [EnumMember(Value = "doc-padm")]
    DocPadm,

    [EnumMember(Value = "doc-padi")]
    DocPadi,

    [EnumMember(Value = "doc-ppni")]
    DocPpni,

    [EnumMember(Value = "doc-pf")]
    DocPf,

    [EnumMember(Value = "doc-po")]
    DocPo,

    [EnumMember(Value = "doc-ncevd")]
    DocNcevd,

    [EnumMember(Value = "doc-ncpqefi")]
    DocNcpqefi,

    [EnumMember(Value = "doc-ncpqebec")]
    DocNcpqebec,

    [EnumMember(Value = "doc-ncts")]
    DocNcts,

    [EnumMember(Value = "doc-nco")]
    DocNco,

    [EnumMember(Value = "doc-orii")]
    DocOrii,

    [EnumMember(Value = "doc-orsr")]
    DocOrsr,

    [EnumMember(Value = "ori-orrnu")]
    OriOrrnu,

    [EnumMember(Value = "phy-orpp")]
    PhyOrpp,

    [EnumMember(Value = "phy-orho")]
    PhyOrho,

    [EnumMember(Value = "phy-is")]
    PhyIs,

    [EnumMember(Value = "phy-orsr")]
    PhyOrsr,

    [EnumMember(Value = "oth-cnl")]
    OthCnl,

    [EnumMember(Value = "oth-o")]
    OthO,
}
