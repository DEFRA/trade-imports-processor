using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ExternalReferenceSystem
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
