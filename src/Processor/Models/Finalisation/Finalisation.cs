using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

namespace Defra.TradeImportsProcessor.Processor.Models.Finalisation;

public class Finalisation
{
    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; set; }

    [JsonPropertyName("header")]
    public required FinalisationHeader Header { get; set; }
}

public class FinalisationHeader
{
    [JsonPropertyName("entryReference")]
    public required string EntryReference { get; set; }

    [JsonPropertyName("entryVersionNumber")]
    public required int EntryVersionNumber { get; set; }

    [JsonPropertyName("decisionNumber")]
    public int? DecisionNumber { get; set; }

    [JsonPropertyName("finalState")]
    public required string FinalState { get; set; }

    [JsonPropertyName("manualAction")]
    public required string ManualAction { get; set; }
}
