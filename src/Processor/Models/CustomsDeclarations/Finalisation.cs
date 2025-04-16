using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class Finalisation
{
    [JsonPropertyName("header")]
    public required FinalisationHeader Header { get; set; }
}
