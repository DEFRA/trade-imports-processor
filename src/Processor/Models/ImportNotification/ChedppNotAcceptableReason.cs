using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Information about not acceptable reason
/// </summary>
public class ChedppNotAcceptableReason
{
    /// <summary>
    ///     reason for refusal
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    ///     commodity or package
    /// </summary>
    [JsonPropertyName("commodityOrPackage")]
    public string? CommodityOrPackage { get; set; }
}
