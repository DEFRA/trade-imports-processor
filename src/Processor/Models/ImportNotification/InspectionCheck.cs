using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class InspectionCheck
{
    /// <summary>
    ///     Type of check
    /// </summary>
    [JsonPropertyName("type")]
    public InspectionCheckType? Type { get; set; }

    /// <summary>
    ///     Status of the check
    /// </summary>
    [JsonPropertyName("status")]
    public InspectionCheckStatus? Status { get; set; }

    /// <summary>
    ///     Reason for the status if applicable
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    ///     Other reason text when selected reason is &#x27;Other&#x27;
    /// </summary>
    [JsonPropertyName("otherReason")]
    public string? OtherReason { get; set; }

    /// <summary>
    ///     Has commodity been selected for checks?
    /// </summary>
    [JsonPropertyName("isSelectedForChecks")]
    public bool? IsSelectedForChecks { get; set; }

    /// <summary>
    ///     Has commodity completed this type of check
    /// </summary>
    [JsonPropertyName("hasChecksComplete")]
    public bool? HasChecksComplete { get; set; }
}
