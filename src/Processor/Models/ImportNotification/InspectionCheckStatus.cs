using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum InspectionCheckStatus
{
    [EnumMember(Value = "To do")]
    ToDo,

    [EnumMember(Value = "Compliant")]
    Compliant,

    [EnumMember(Value = "Auto cleared")]
    AutoCleared,

    [EnumMember(Value = "Non compliant")]
    NonCompliant,

    [EnumMember(Value = "Not inspected")]
    NotInspected,

    [EnumMember(Value = "To be inspected")]
    ToBeInspected,

    [EnumMember(Value = "Hold")]
    Hold,
}
