
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum JourneyRiskCategorisationResultRiskLevelEnum
{

    [EnumMember(Value = "High")]
    High,

    [EnumMember(Value = "Medium")]
    Medium,

    [EnumMember(Value = "Low")]
    Low,

}