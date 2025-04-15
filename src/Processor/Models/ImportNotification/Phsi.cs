using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     PHSI Decision Breakdown
/// </summary>
public class Phsi
{
    /// <summary>
    ///     Whether or not a documentary check is required for PHSI
    /// </summary>
    [JsonPropertyName("documentCheck")]
    public bool? DocumentCheck { get; set; }

    /// <summary>
    ///     Whether or not an identity check is required for PHSI
    /// </summary>
    [JsonPropertyName("identityCheck")]
    public bool? IdentityCheck { get; set; }

    /// <summary>
    ///     Whether or not a physical check is required for PHSI
    /// </summary>
    [JsonPropertyName("physicalCheck")]
    public bool? PhysicalCheck { get; set; }
}
