using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundErrorItem
{
    [JsonPropertyName("errorCode")]
    public required string errorCode { get; init; }

    [JsonPropertyName("errorMessage")]
    public required string errorMessage { get; init; }
}
