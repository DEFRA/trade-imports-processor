#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Seal container details
/// </summary>
public partial class SealContainer
{
    [JsonPropertyName("sealNumber")]
    public string? SealNumber { get; set; }

    [JsonPropertyName("containerNumber")]
    public string? ContainerNumber { get; set; }

    [JsonPropertyName("officialSeal")]
    public bool? OfficialSeal { get; set; }

    [JsonPropertyName("resealedSealNumber")]
    public string? ResealedSealNumber { get; set; }
}
