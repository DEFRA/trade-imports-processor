using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

public class ClearanceRequest
{
    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("header")]
    public required Header Header { get; init; }

    [JsonPropertyName("items")]
    public required Items[] Items { get; init; }
}
