using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PartOneProvideCtcMrn
{
    [EnumMember(Value = "YES")]
    Yes,

    [EnumMember(Value = "YES_ADD_LATER")]
    YesAddLater,

    [EnumMember(Value = "NO")]
    No,
}
