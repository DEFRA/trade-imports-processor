using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class Header
{
    [JsonPropertyName("entryReference")]
    public required string EntryReference { get; init; }

    [JsonPropertyName("entryVersionNumber")]
    public int? EntryVersionNumber { get; init; }

    [JsonPropertyName("decisionNumber")]
    public int? DecisionNumber { get; init; }
}
