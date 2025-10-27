using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class GmrActualCrossing
{
    [JsonPropertyName("localDateTimeOfArrival")]
    public DateTime? LocalDateTimeOfArrival { get; init; }

    [JsonPropertyName("routeId")]
    public string? RouteId { get; init; }
}
