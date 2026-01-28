using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public record MatchedGmr
{
    [JsonPropertyName("mrn")]
    public string? Mrn { get; init; }

    [JsonPropertyName("gmr")]
    public required Gmr Gmr { get; init; }

    public string GetIdentifier => $"{Mrn ?? "Unknown"}-{Gmr.GmrId}";
}
