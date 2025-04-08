
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommodityRiskResultPhsiClassificationEnum
{

    [EnumMember(Value = "Mandatory")]
    Mandatory,

    [EnumMember(Value = "Reduced")]
    Reduced,

    [EnumMember(Value = "Controlled")]
    Controlled,

}