
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ChedppNotAcceptableReasonCommodityOrPackageEnum
{

    [EnumMember(Value = "c")]
    C,

    [EnumMember(Value = "p")]
    P,

    [EnumMember(Value = "cp")]
    Cp,

}