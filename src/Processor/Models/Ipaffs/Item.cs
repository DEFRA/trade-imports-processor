using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class Item
{
    [JsonPropertyName("itemNumber")]
    public int? ItemNumber { get; set; }

    [JsonPropertyName("checks")]
    public Check[]? Checks { get; set; }
}
