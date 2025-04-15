using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

public class ClearanceRequestPostResult
{
    [JsonPropertyName("xmlSchemaVersion")]
    public string? XmlSchemaVersion { get; set; }

    [JsonPropertyName("sendingDate")]
    [EpochDateTimeJsonConverter]
    public DateTime? SendingDate { get; set; }

    [JsonPropertyName("operationCode")]
    public int? OperationCode { get; set; }

    [JsonPropertyName("requestIdentifier")]
    public string? RequestIdentifier { get; set; }
}
