using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class Document
{
    [JsonPropertyName("documentCode")]
    public string? DocumentCode { get; set; }

    [JsonPropertyName("documentReference")]
    public string? DocumentReference { get; set; }

    [JsonPropertyName("documentStatus")]
    public string? DocumentStatus { get; set; }

    [JsonPropertyName("documentControl")]
    public string? DocumentControl { get; set; }

    [JsonPropertyName("documentQuantity")]
    public decimal? DocumentQuantity { get; set; }
}
