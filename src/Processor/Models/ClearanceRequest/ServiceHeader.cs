using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

public class ServiceHeader
{
    [JsonPropertyName("sourceSystem")]
    public required string? SourceSystem { get; set; }

    [JsonPropertyName("destinationSystem")]
    public required string? DestinationSystem { get; set; }

    [JsonPropertyName("correlationId")]
    public required string CorrelationId { get; set; }

    [JsonPropertyName("serviceCallTimestamp")]
    [EpochDateTimeJsonConverter]
    public required DateTime ServiceCallTimestamp { get; set; }
}
