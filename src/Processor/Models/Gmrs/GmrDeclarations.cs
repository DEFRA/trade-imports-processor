using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class GmrDeclarations
{
    [JsonPropertyName("customs")]
    public GmrDeclaration[]? Customs { get; init; }

    [JsonPropertyName("transits")]
    public GmrDeclaration[]? Transits { get; init; }
}
