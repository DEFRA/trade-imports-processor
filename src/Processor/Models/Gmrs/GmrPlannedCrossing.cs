using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class GmrPlannedCrossing
{
    [JsonPropertyName("localDateTimeOfDeparture")]
    public DateTime? LocalDateTimeOfDeparture { get; init; }

    [JsonPropertyName("routeId")]
    public string? RouteId { get; init; }
}
