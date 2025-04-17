using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundError
{
    [JsonPropertyName("header")]
    public required Header Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("errors")]
    public required InboundErrorItem[] Errors { get; init; }
}
