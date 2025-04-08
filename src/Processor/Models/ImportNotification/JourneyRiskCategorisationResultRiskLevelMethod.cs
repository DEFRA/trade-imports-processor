using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum JourneyRiskCategorisationResultRiskLevelMethod
{
    [EnumMember(Value = "System")]
    System,

    [EnumMember(Value = "User")]
    User,
}
