
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionIfChanneledOptionEnum
{

    [EnumMember(Value = "article8")]
    Article8,

    [EnumMember(Value = "article15")]
    Article15,

}