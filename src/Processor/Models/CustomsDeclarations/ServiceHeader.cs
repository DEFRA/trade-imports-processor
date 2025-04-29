using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class ServiceHeader
{
    [JsonPropertyName("sourceSystem")]
    public required string SourceSystem { get; init; }

    [JsonPropertyName("destinationSystem")]
    public required string DestinationSystem { get; init; }

    [JsonPropertyName("correlationId")]
    public required string CorrelationId { get; init; }

    [JsonPropertyName("serviceCallTimestamp")]
    [EpochDateTimeJsonConverter]
    public required DateTime ServiceCallTimestamp { get; init; }
}
