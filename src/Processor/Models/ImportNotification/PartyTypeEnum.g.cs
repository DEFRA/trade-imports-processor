
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PartyTypeEnum
{

    [EnumMember(Value = "Commercial transporter")]
    CommercialTransporter,

    [EnumMember(Value = "Private transporter")]
    PrivateTransporter,

}