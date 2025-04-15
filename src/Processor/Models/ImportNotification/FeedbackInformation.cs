using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Feedback information from control
/// </summary>
public class FeedbackInformation
{
    /// <summary>
    ///     Type of authority
    /// </summary>
    [JsonPropertyName("authorityType")]
    public FeedbackInformationAuthorityType? AuthorityType { get; set; }

    /// <summary>
    ///     Did the consignment arrive
    /// </summary>
    [JsonPropertyName("consignmentArrival")]
    public bool? ConsignmentArrival { get; set; }

    /// <summary>
    ///     Does the consignment conform
    /// </summary>
    [JsonPropertyName("consignmentConformity")]
    public bool? ConsignmentConformity { get; set; }

    /// <summary>
    ///     Reason for consignment not arriving at the entry point
    /// </summary>
    [JsonPropertyName("consignmentNoArrivalReason")]
    public string? ConsignmentNoArrivalReason { get; set; }

    /// <summary>
    ///     Date of consignment destruction
    /// </summary>
    [JsonPropertyName("destructionDate")]
    public string? DestructionDate { get; set; }
}
