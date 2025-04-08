using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ConsignmentCheckIdentityCheckType
{
    [EnumMember(Value = "Seal Check")]
    SealCheck,

    [EnumMember(Value = "Full Identity Check")]
    FullIdentityCheck,

    [EnumMember(Value = "Not Done")]
    NotDone,
}
