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
    public ChedppNotAcceptableReasonReason? Reason { get; set; }

    /// <summary>
    ///     commodity or package
    /// </summary>
    [JsonPropertyName("commodityOrPackage")]
    public ChedppNotAcceptableReasonCommodityOrPackage? CommodityOrPackage { get; set; }
}
