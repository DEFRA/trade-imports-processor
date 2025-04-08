using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionNotAcceptableActionIndustrialProcessingReason
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
