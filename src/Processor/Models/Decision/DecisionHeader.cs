using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

public class DecisionHeader
{
    /// <summary>
    /// </summary>
    [JsonPropertyName("decisionNumber")]
    public int? DecisionNumber { get; set; }
}
