using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class ImportNotification //: CustomStringMongoIdentifiable
{
    /// <summary>
    ///     The IPAFFS ID number for this notification.
    /// </summary>
    [JsonPropertyName("id")]
    public int? IpaffsId { get; set; }

    /// <summary>
    ///     The etag for this notification.
    /// </summary>
    [JsonPropertyName("etag")]
    public string? Etag { get; set; }

    /// <summary>
    ///     List of external references, which relate to downstream services
    /// </summary>
    [JsonPropertyName("externalReferences")]
    public ExternalReference[]? ExternalReferences { get; set; }

    /// <summary>
    ///     Reference number of the notification
    /// </summary>
    [JsonPropertyName("referenceNumber")]
    [Required]
    public required string ReferenceNumber { get; set; }

    /// <summary>
    ///     Current version of the notification
    /// </summary>
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    /// <summary>
    ///     Date when the notification was last updated.
    /// </summary>
    [JsonPropertyName("lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    ///     User entity whose update was last
    /// </summary>
    [JsonPropertyName("lastUpdatedBy")]
    public UserInformation? LastUpdatedBy { get; set; }

    /// <summary>
    ///     The Type of notification that has been submitted
    /// </summary>
    [JsonPropertyName("type")]
    public ImportNotificationType? ImportNotificationType { get; set; }

    /// <summary>
    ///     Reference number of notification that was replaced by this one
    /// </summary>
    [JsonPropertyName("replaces")]
    public string? Replaces { get; set; }

    /// <summary>
    ///     Reference number of notification that replaced this one
    /// </summary>
    [JsonPropertyName("replacedBy")]
    public string? ReplacedBy { get; set; }

    /// <summary>
    ///     Current status of the notification. When created by an importer, the notification has the status &#x27;SUBMITTED
    ///     &#x27;. Before submission of the notification it has the status &#x27;DRAFT&#x27;. When the BIP starts validation
    ///     of the notification it has the status &#x27;IN PROGRESS&#x27; Once the BIP validates the notification, it gets the
    ///     status &#x27;VALIDATED&#x27;. &#x27;AMEND&#x27; is set when the Part-1 user is modifying the notification. &#x27;
    ///     MODIFY&#x27; is set when Part-2 user is modifying the notification. Replaced - When the notification is replaced by
    ///     another notification. Rejected - Notification moves to Rejected status when rejected by a Part-2 user.
    /// </summary>
    [JsonPropertyName("status")]
    public ImportNotificationStatus? Status { get; set; }

    /// <summary>
    ///     Present if the consignment has been split
    /// </summary>
    [JsonPropertyName("splitConsignment")]
    public SplitConsignment? SplitConsignment { get; set; }

    /// <summary>
    ///     Is this notification a child of a split consignment?
    /// </summary>
    [JsonPropertyName("childNotification")]
    public bool? ChildNotification { get; set; }

    /// <summary>
    ///     Result of risk assessment by the risk scorer
    /// </summary>
    [JsonPropertyName("riskAssessment")]
    public RiskAssessmentResult? RiskAssessment { get; set; }

    /// <summary>
    ///     Details of the risk categorisation level for a notification
    /// </summary>
    [JsonPropertyName("journeyRiskCategorisation")]
    public JourneyRiskCategorisationResult? JourneyRiskCategorisation { get; set; }

    /// <summary>
    ///     Is this notification a high risk notification from the EU/EEA?
    /// </summary>
    [JsonPropertyName("isHighRiskEuImport")]
    public bool? IsHighRiskEuImport { get; set; }

    [JsonPropertyName("partOne")]
    public PartOne? PartOne { get; set; }

    /// <summary>
    ///     Information about the user who set the decision in Part 2
    /// </summary>
    [JsonPropertyName("decisionBy")]
    public UserInformation? DecisionBy { get; set; }

    /// <summary>
    ///     Date when the notification was validated or rejected
    /// </summary>
    [JsonPropertyName("decisionDate")]
    public string? DecisionDate { get; set; }

    /// <summary>
    ///     Part of the notification which contains information filled by inspector at BIP/DPE
    /// </summary>
    [JsonPropertyName("partTwo")]
    public PartTwo? PartTwo { get; set; }

    /// <summary>
    ///     Part of the notification which contains information filled by LVU if control of consignment is needed.
    /// </summary>
    [JsonPropertyName("partThree")]
    public PartThree? PartThree { get; set; }

    /// <summary>
    ///     Official veterinarian
    /// </summary>
    [JsonPropertyName("officialVeterinarian")]
    public string? OfficialVeterinarian { get; set; }

    /// <summary>
    ///     Validation messages for whole notification
    /// </summary>
    [JsonPropertyName("consignmentValidation")]
    public ValidationMessageCode[]? ConsignmentValidations { get; set; }

    /// <summary>
    ///     Organisation id which the agent user belongs to, stored against each notification which has been raised on behalf
    ///     of another organisation
    /// </summary>
    [JsonPropertyName("agencyOrganisationId")]
    public string? AgencyOrganisationId { get; set; }

    /// <summary>
    ///     Date and Time when risk decision was locked
    /// </summary>
    [JsonPropertyName("riskDecisionLockingTime")]
    public DateTime? RiskDecisionLockedOn { get; set; }

    /// <summary>
    ///     is the risk decision locked?
    /// </summary>
    [JsonPropertyName("isRiskDecisionLocked")]
    public bool? IsRiskDecisionLocked { get; set; }

    /// <summary>
    ///     Boolean flag that indicates whether a bulk upload is in progress
    /// </summary>
    [JsonPropertyName("isBulkUploadInProgress")]
    public bool? IsBulkUploadInProgress { get; set; }

    /// <summary>
    ///     Request UUID to trace bulk upload
    /// </summary>
    [JsonPropertyName("requestId")]
    public string? RequestId { get; set; }

    /// <summary>
    ///     Have all commodities been matched with corresponding CDS declaration(s)
    /// </summary>
    [JsonPropertyName("isCdsFullMatched")]
    public bool? IsCdsFullMatched { get; set; }

    /// <summary>
    ///     The version of the ched type the notification was created with
    /// </summary>
    [JsonPropertyName("chedTypeVersion")]
    public int? ChedTypeVersion { get; set; }

    /// <summary>
    ///     Indicates whether a CHED has been matched with a GVMS GMR via DMP
    /// </summary>
    [JsonPropertyName("isGMRMatched")]
    public bool? IsGMRMatched { get; set; }
}
