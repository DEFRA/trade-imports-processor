using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionNotAcceptableActionEntryRefusalReason
{
    [EnumMember(Value = "ContaminatedProducts")]
    ContaminatedProducts,

    [EnumMember(Value = "InterceptedPart")]
    InterceptedPart,

    [EnumMember(Value = "PackagingMaterial")]
    PackagingMaterial,

    [EnumMember(Value = "MeansOfTransport")]
    MeansOfTransport,

    [EnumMember(Value = "Other")]
    Other,
}
