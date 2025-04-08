using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommoditiesCommodityIntendedFor
{
    [EnumMember(Value = "human")]
    Human,

    [EnumMember(Value = "feedingstuff")]
    Feedingstuff,

    [EnumMember(Value = "further")]
    Further,

    [EnumMember(Value = "other")]
    Other,
}
