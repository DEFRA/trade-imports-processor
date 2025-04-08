using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ChedppNotAcceptableReasonCommodityOrPackage
{
    [EnumMember(Value = "c")]
    C,

    [EnumMember(Value = "p")]
    P,

    [EnumMember(Value = "cp")]
    Cp,
}
