using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ImportNotificationType
{
    [EnumMember(Value = "CVEDA")]
    Cveda,

    [EnumMember(Value = "CVEDP")]
    Cvedp,

    [EnumMember(Value = "CHEDPP")]
    Chedpp,

    [EnumMember(Value = "CED")]
    Ced,

    [EnumMember(Value = "IMP")]
    Imp,
}
