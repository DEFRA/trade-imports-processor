using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum CommodityRiskResultRiskDecision
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
