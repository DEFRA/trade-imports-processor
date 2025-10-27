using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class GmrDeclaration
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }
}
