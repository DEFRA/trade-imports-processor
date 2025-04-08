using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum JourneyRiskCategorisationResultRiskLevel
{
    [EnumMember(Value = "High")]
    High,

    [EnumMember(Value = "Medium")]
    Medium,

    [EnumMember(Value = "Low")]
    Low,
}
