using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class ClearanceRequestPost
{
    [JsonPropertyName("xmlSchemaVersion")]
    public string? XmlSchemaVersion { get; set; }

    [JsonPropertyName("userIdentification")]
    public string? UserIdentification { get; set; }

    [JsonPropertyName("userPassword")]
    public string? UserPassword { get; set; }

    [JsonPropertyName("sendingDate")]
    [EpochDateTimeJsonConverter]
    public DateTime? SendingDate { get; set; }

    [JsonPropertyName("alvsClearanceRequest")]
    public ClearanceRequest? AlvsClearanceRequest { get; set; }
}
