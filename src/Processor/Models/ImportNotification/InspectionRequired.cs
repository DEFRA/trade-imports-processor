using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum InspectionRequired
{
    [EnumMember(Value = "Required")]
    Required,

    [EnumMember(Value = "Inconclusive")]
    Inconclusive,

    [EnumMember(Value = "Not required")]
    NotRequired,
}
