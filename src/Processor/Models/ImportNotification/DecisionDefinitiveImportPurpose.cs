using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionDefinitiveImportPurpose
{
    [EnumMember(Value = "slaughter")]
    Slaughter,

    [EnumMember(Value = "approvedbodies")]
    Approvedbodies,

    [EnumMember(Value = "quarantine")]
    Quarantine,
}
