
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ExternalReferenceSystemEnum
{

    [EnumMember(Value = "E-CERT")]
    Ecert,

    [EnumMember(Value = "E-PHYTO")]
    Ephyto,

    [EnumMember(Value = "E-NOTIFICATION")]
    Enotification,

    [EnumMember(Value = "NCTS")]
    Ncts,

}