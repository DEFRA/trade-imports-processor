using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class Check
{
    [JsonPropertyName("checkCode")]
    public string? CheckCode { get; set; }

    [JsonPropertyName("decisionCode")]
    public string? DecisionCode { get; set; }

    [JsonPropertyName("decisionValidUntil")]
    public string? DecisionValidUntil { get; set; }

    [JsonPropertyName("decisionReason")]
    public string[]? DecisionReason { get; set; }
}
