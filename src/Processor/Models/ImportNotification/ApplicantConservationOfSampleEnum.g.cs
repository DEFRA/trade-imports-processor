
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum ApplicantConservationOfSampleEnum
{

    [EnumMember(Value = "Ambient")]
    Ambient,

    [EnumMember(Value = "Chilled")]
    Chilled,

    [EnumMember(Value = "Frozen")]
    Frozen,

}