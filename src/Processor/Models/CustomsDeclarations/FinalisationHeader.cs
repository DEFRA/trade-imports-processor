using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class FinalisationHeader : Header
{
    [JsonPropertyName("decisionNumber")]
    public int? DecisionNumber { get; set; }

    [JsonPropertyName("finalState")]
    public required string FinalState { get; set; }

    [JsonPropertyName("manualAction")]
    public required string ManualAction { get; set; }
}
