using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class GmrActualCrossing
{
    public DateTime? LocalDateTimeOfArrival { get; init; }
    public string? RouteId { get; init; }
}
