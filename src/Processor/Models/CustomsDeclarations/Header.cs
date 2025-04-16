using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class Header
{
    [JsonPropertyName("entryReference")]
    public string? EntryReference { get; set; }

    [JsonPropertyName("entryVersionNumber")]
    public int? EntryVersionNumber { get; set; }
}
