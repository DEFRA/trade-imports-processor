
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum InspectionCheckTypeEnum
{

    [EnumMember(Value = "PHSI_DOCUMENT")]
    PhsiDocument,

    [EnumMember(Value = "PHSI_IDENTITY")]
    PhsiIdentity,

    [EnumMember(Value = "PHSI_PHYSICAL")]
    PhsiPhysical,

    [EnumMember(Value = "HMI")]
    Hmi,

}