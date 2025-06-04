using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundErrorHeader : Header
{
    [JsonPropertyName("sourceCorrelationId")]
    public required string SourceCorrelationId { get; init; }
}
