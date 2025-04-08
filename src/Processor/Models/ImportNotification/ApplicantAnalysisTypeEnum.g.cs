
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ApplicantAnalysisTypeEnum
{

    [EnumMember(Value = "Initial analysis")]
    InitialAnalysis,

    [EnumMember(Value = "Counter analysis")]
    CounterAnalysis,

    [EnumMember(Value = "Second expert analysis")]
    SecondExpertAnalysis,

}