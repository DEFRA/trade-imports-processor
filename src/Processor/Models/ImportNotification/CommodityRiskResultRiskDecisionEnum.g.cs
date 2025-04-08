
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommodityRiskResultRiskDecisionEnum
{

    [EnumMember(Value = "REQUIRED")]
    Required,

    [EnumMember(Value = "NOTREQUIRED")]
    Notrequired,

    [EnumMember(Value = "INCONCLUSIVE")]
    Inconclusive,

    [EnumMember(Value = "REENFORCED_CHECK")]
    ReenforcedCheck,

}