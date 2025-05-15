using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class ImportNotification
{
    [JsonPropertyName("id")]
    public int? IpaffsId { get; init; }

    [JsonPropertyName("etag")]
    public string? Etag { get; init; }

    [JsonPropertyName("externalReferences")]
    public ExternalReference[]? ExternalReferences { get; init; }

    [JsonPropertyName("referenceNumber")]
    [Required]
    public required string ReferenceNumber { get; init; }

    [JsonPropertyName("version")]
    public int? Version { get; init; }

    [JsonPropertyName("lastUpdated")]
    public DateTime? LastUpdated { get; init; }

    [JsonPropertyName("lastUpdatedBy")]
    public UserInformation? LastUpdatedBy { get; init; }

    [JsonPropertyName("type")]
    public string? ImportNotificationType { get; init; }

    [JsonPropertyName("replaces")]
    public string? Replaces { get; init; }

    [JsonPropertyName("replacedBy")]
    public string? ReplacedBy { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("splitConsignment")]
    public SplitConsignment? SplitConsignment { get; init; }

    [JsonPropertyName("childNotification")]
    public bool? ChildNotification { get; init; }

    [JsonPropertyName("riskAssessment")]
    public RiskAssessmentResult? RiskAssessment { get; init; }

    [JsonPropertyName("journeyRiskCategorisation")]
    public JourneyRiskCategorisationResult? JourneyRiskCategorisation { get; init; }

    [JsonPropertyName("isHighRiskEuImport")]
    public bool? IsHighRiskEuImport { get; init; }

    [JsonPropertyName("partOne")]
    public PartOne? PartOne { get; init; }

    [JsonPropertyName("decisionBy")]
    public UserInformation? DecisionBy { get; init; }

    [JsonPropertyName("decisionDate")]
    public string? DecisionDate { get; init; }

    [JsonPropertyName("partTwo")]
    public PartTwo? PartTwo { get; init; }

    [JsonPropertyName("partThree")]
    public PartThree? PartThree { get; init; }

    [JsonPropertyName("officialVeterinarian")]
    public string? OfficialVeterinarian { get; init; }

    [JsonPropertyName("consignmentValidation")]
    public ValidationMessageCode[]? ConsignmentValidations { get; init; }

    [JsonPropertyName("agencyOrganisationId")]
    public string? AgencyOrganisationId { get; init; }

    [JsonPropertyName("riskDecisionLockingTime")]
    public DateTime? RiskDecisionLockedOn { get; init; }

    [JsonPropertyName("isRiskDecisionLocked")]
    public bool? IsRiskDecisionLocked { get; init; }

    [JsonPropertyName("isAutoClearanceExempted")]
    public bool? IsAutoClearanceExempted { get; init; }

    [JsonPropertyName("isBulkUploadInProgress")]
    public bool? IsBulkUploadInProgress { get; init; }

    [JsonPropertyName("requestId")]
    public string? RequestId { get; init; }

    [JsonPropertyName("isCdsFullMatched")]
    public bool? IsCdsFullMatched { get; init; }

    [JsonPropertyName("chedTypeVersion")]
    public int? ChedTypeVersion { get; init; }

    [JsonPropertyName("isGMRMatched")]
    public bool? IsGMRMatched { get; init; }

    public static explicit operator DataApiIpaffs.ImportPreNotification(ImportNotification importNotification)
    {
        return new DataApiIpaffs.ImportPreNotification
        {
            IpaffsId = importNotification.IpaffsId,
            Etag = importNotification.Etag,
            ExternalReferences = importNotification.ExternalReferences?.Select(ExternalReferenceMapper.Map).ToArray(),
            ReferenceNumber = importNotification.ReferenceNumber,
            Version = importNotification.Version,
            UpdatedSource = importNotification.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(importNotification.LastUpdatedBy),
            ImportNotificationType = importNotification.ImportNotificationType,
            Replaces = importNotification.Replaces,
            ReplacedBy = importNotification.ReplacedBy,
            Status = importNotification.Status,
            SplitConsignment = SplitConsignmentMapper.Map(importNotification.SplitConsignment),
            ChildNotification = importNotification.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(
                importNotification.JourneyRiskCategorisation
            ),
            IsHighRiskEuImport = importNotification.IsHighRiskEuImport,
            DecisionBy = UserInformationMapper.Map(importNotification.DecisionBy),
            DecisionDate = importNotification.DecisionDate,
            PartOne = PartOneMapper.Map(importNotification.PartOne),
            PartTwo = PartTwoMapper.Map(importNotification.PartTwo),
            PartThree = PartThreeMapper.Map(importNotification.PartThree),
            OfficialVeterinarian = importNotification.OfficialVeterinarian,
            ConsignmentValidations = importNotification
                .ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map)
                .ToArray(),
            AgencyOrganisationId = importNotification.AgencyOrganisationId,
            RiskDecisionLockedOn = importNotification.RiskDecisionLockedOn,
            IsRiskDecisionLocked = importNotification.IsRiskDecisionLocked,
            IsAutoClearanceExempted = importNotification.IsAutoClearanceExempted,
            IsBulkUploadInProgress = importNotification.IsBulkUploadInProgress,
            RequestId = importNotification.RequestId,
            IsCdsFullMatched = importNotification.IsCdsFullMatched,
            ChedTypeVersion = importNotification.ChedTypeVersion,
            IsGMRMatched = importNotification.IsGMRMatched,
            RiskAssessment = RiskAssessmentResultMapper.Map(importNotification.RiskAssessment),
        };
    }
}
