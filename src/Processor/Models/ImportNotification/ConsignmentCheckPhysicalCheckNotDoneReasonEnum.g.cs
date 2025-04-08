
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ConsignmentCheckPhysicalCheckNotDoneReasonEnum
{

    [EnumMember(Value = "Reduced checks regime")]
    ReducedChecksRegime,

    [EnumMember(Value = "Other")]
    Other,

}