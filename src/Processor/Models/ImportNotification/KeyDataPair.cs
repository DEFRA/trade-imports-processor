#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class KeyDataPair
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}
