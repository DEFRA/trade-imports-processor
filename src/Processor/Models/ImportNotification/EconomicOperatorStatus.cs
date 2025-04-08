using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum EconomicOperatorStatus
{
    [EnumMember(Value = "approved")]
    Approved,

    [EnumMember(Value = "nonapproved")]
    Nonapproved,

    [EnumMember(Value = "suspended")]
    Suspended,
}
