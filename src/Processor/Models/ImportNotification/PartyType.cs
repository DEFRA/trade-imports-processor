using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PartyType
{
    [EnumMember(Value = "Commercial transporter")]
    CommercialTransporter,

    [EnumMember(Value = "Private transporter")]
    PrivateTransporter,
}
