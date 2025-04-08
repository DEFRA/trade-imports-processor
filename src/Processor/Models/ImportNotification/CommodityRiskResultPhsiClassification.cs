using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommodityRiskResultPhsiClassification
{
    [EnumMember(Value = "Mandatory")]
    Mandatory,

    [EnumMember(Value = "Reduced")]
    Reduced,

    [EnumMember(Value = "Controlled")]
    Controlled,
}
