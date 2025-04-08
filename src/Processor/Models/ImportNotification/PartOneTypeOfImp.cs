using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PartOneTypeOfImp
{
    [EnumMember(Value = "A")]
    A,

    [EnumMember(Value = "P")]
    P,

    [EnumMember(Value = "D")]
    D,
}
