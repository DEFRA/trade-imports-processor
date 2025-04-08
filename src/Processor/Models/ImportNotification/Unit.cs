using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum Unit
{
    [EnumMember(Value = "percent")]
    Percent,

    [EnumMember(Value = "number")]
    Number,
}
