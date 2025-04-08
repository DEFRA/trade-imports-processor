using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ConsignmentCheckIdentityCheckNotDoneReason
{
    [EnumMember(Value = "Reduced checks regime")]
    ReducedChecksRegime,

    [EnumMember(Value = "Not required")]
    NotRequired,
}
