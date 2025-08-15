using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class FinalisationHeader
{
    [JsonPropertyName("entryReference")]
    public required string EntryReference { get; init; }

    [JsonPropertyName("entryVersionNumber")]
    public int? EntryVersionNumber { get; init; }

    [JsonPropertyName("decisionNumber")]
    public int? DecisionNumber { get; set; }

    [JsonPropertyName("finalState")]
    public required string FinalState { get; set; }

    [JsonPropertyName("manualAction")]
    public required string ManualAction { get; set; }
}
