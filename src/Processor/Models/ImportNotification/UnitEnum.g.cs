
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum UnitEnum
{

    [EnumMember(Value = "percent")]
    Percent,

    [EnumMember(Value = "number")]
    Number,

}