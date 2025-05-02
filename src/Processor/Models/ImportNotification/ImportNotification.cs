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
    public ImportNotificationType? ImportNotificationType { get; init; }

    [JsonPropertyName("replaces")]
    public string? Replaces { get; init; }

    [JsonPropertyName("replacedBy")]
    public string? ReplacedBy { get; init; }

    [JsonPropertyName("status")]
    public ImportNotificationStatus? Status { get; init; }

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

    private static string FromSnakeCase(string input)
    {
        if (input == "netweight")
        {
            return "netWeight";
        }

        var pascal = input
            .Split(["_"], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
        return char.ToLower(pascal[0]) + pascal[1..];
    }

    private static IDictionary<string, object> FromSnakeCase(IDictionary<string, object>? input)
    {
        return input == null
            ? new Dictionary<string, object>()
            : input.ToDictionary(mc => FromSnakeCase(mc.Key), mc => mc.Value);
    }

    public static explicit operator DataApiIpaffs.ImportPreNotification(ImportNotification importNotification)
    {
        var commodities = importNotification.PartOne?.Commodities;

        if (commodities?.CommodityComplements?.Length == 1)
        {
            commodities.CommodityComplements[0].AdditionalData = FromSnakeCase(
                commodities.ComplementParameterSets![0].KeyDataPairs!
            );
            if (importNotification.RiskAssessment != null)
            {
                commodities.CommodityComplements[0].RiskAssesment = importNotification.RiskAssessment.CommodityResults![
                    0
                ];
            }
        }
        else
        {
            var complementParameters = new Dictionary<int, ComplementParameterSet>();
            var complementRiskAssessments = new Dictionary<string, CommodityRiskResult>();
            var commodityChecks = new Dictionary<string, InspectionCheck[]>();

            if (commodities?.ComplementParameterSets != null)
            {
                foreach (var commoditiesCommodityComplement in commodities.ComplementParameterSets)
                {
                    complementParameters[commoditiesCommodityComplement.ComplementId!.Value] =
                        commoditiesCommodityComplement;
                }
            }

            if (importNotification.RiskAssessment?.CommodityResults != null)
            {
                foreach (var commoditiesRa in importNotification.RiskAssessment.CommodityResults)
                {
                    complementRiskAssessments[commoditiesRa.UniqueId!] = commoditiesRa;
                }
            }

            if (importNotification.PartTwo?.CommodityChecks != null)
            {
                foreach (var commodityCheck in importNotification.PartTwo.CommodityChecks!)
                {
                    commodityChecks[commodityCheck.UniqueComplementId!] = commodityCheck.Checks!;
                }
            }

            if (commodities?.CommodityComplements is not null)
            {
                foreach (var commodity in commodities.CommodityComplements)
                {
                    var parameters = complementParameters[commodity.ComplementId!.Value];
                    commodity.AdditionalData = FromSnakeCase(parameters.KeyDataPairs!);

                    if (
                        complementRiskAssessments.Count != 0
                        && parameters.UniqueComplementId is not null
                        && complementRiskAssessments.TryGetValue(
                            parameters.UniqueComplementId,
                            out var riskAssessmentValue
                        )
                    )
                    {
                        commodity.RiskAssesment = riskAssessmentValue;
                    }

                    if (
                        commodityChecks.Count != 0
                        && parameters.UniqueComplementId is not null
                        && commodityChecks.TryGetValue(parameters.UniqueComplementId, out var checksValue)
                    )
                    {
                        commodity.Checks = checksValue;
                    }
                }
            }
        }

        return new DataApiIpaffs.ImportPreNotification
        {
            IpaffsId = importNotification.IpaffsId,
            Etag = importNotification.Etag,
            ExternalReferences = importNotification.ExternalReferences?.Select(ExternalReferenceMapper.Map).ToArray(),
            ReferenceNumber = importNotification.ReferenceNumber,
            Version = importNotification.Version,
            UpdatedSource = importNotification.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(importNotification.LastUpdatedBy),
            ImportNotificationType = ImportNotificationTypeEnumMapper.Map(importNotification.ImportNotificationType),
            Replaces = importNotification.Replaces,
            ReplacedBy = importNotification.ReplacedBy,
            Status = ImportNotificationStatusEnumMapper.Map(importNotification.Status),
            SplitConsignment = SplitConsignmentMapper.Map(importNotification.SplitConsignment),
            ChildNotification = importNotification.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(
                importNotification.JourneyRiskCategorisation
            ),
            IsHighRiskEuImport = importNotification.IsHighRiskEuImport,
            PartOne = PartOneMapper.Map(importNotification.PartOne),
            DecisionBy = UserInformationMapper.Map(importNotification.DecisionBy),
            DecisionDate = importNotification.DecisionDate,
            PartTwo = PartTwoMapper.Map(importNotification.PartTwo),
            PartThree = PartThreeMapper.Map(importNotification.PartThree),
            OfficialVeterinarian = importNotification.OfficialVeterinarian,
            ConsignmentValidations = importNotification
                .ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map)
                .ToArray(),
            AgencyOrganisationId = importNotification.AgencyOrganisationId,
            RiskDecisionLockedOn = importNotification.RiskDecisionLockedOn,
            IsRiskDecisionLocked = importNotification.IsRiskDecisionLocked,
            IsBulkUploadInProgress = importNotification.IsBulkUploadInProgress,
            RequestId = importNotification.RequestId,
            IsCdsFullMatched = importNotification.IsCdsFullMatched,
            ChedTypeVersion = importNotification.ChedTypeVersion,
            IsGMRMatched = importNotification.IsGMRMatched,
            CommoditiesSummary =
                commodities != null ? CommoditiesMapper.Map(commodities) : new DataApiIpaffs.Commodities(),
            Commodities =
                commodities != null
                    ? commodities.CommodityComplements?.Select(CommodityComplementMapper.Map).ToArray()!
                    : [],
        };
    }
}
