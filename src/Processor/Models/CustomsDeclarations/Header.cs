using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class Header
{
    [JsonPropertyName("entryReference")]
    public required string EntryReference { get; init; }

    [JsonPropertyName("entryVersionNumber")]
    public required int EntryVersionNumber { get; init; }
}
