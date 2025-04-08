
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ControlConsignmentLeaveEnum
{

    [EnumMember(Value = "YES")]
    Yes,

    [EnumMember(Value = "NO")]
    No,

    [EnumMember(Value = "It has been destroyed")]
    ItHasBeenDestroyed,

}