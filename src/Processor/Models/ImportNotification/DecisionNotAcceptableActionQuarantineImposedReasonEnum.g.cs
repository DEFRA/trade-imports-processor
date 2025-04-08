
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionNotAcceptableActionQuarantineImposedReasonEnum
{

    [EnumMember(Value = "ContaminatedProducts")]
    ContaminatedProducts,

    [EnumMember(Value = "InterceptedPart")]
    InterceptedPart,

    [EnumMember(Value = "PackagingMaterial")]
    PackagingMaterial,

    [EnumMember(Value = "Other")]
    Other,

}