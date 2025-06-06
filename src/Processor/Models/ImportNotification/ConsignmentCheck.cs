using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     consignment checks
/// </summary>
public class ConsignmentCheck
{
    /// <summary>
    ///     Does it conform EU standards
    /// </summary>
    [JsonPropertyName("euStandard")]
    public string? EuStandard { get; set; }

    /// <summary>
    ///     Result of additional guarantees
    /// </summary>
    [JsonPropertyName("additionalGuarantees")]
    public string? AdditionalGuarantees { get; set; }

    [JsonPropertyName("documentCheckAdditionalDetails")]
    public string? DocumentCheckAdditionalDetails { get; set; }

    /// <summary>
    ///     Result of document check
    /// </summary>
    [JsonPropertyName("documentCheckResult")]
    public string? DocumentCheckResult { get; set; }

    /// <summary>
    ///     Result of national requirements check
    /// </summary>
    [JsonPropertyName("nationalRequirements")]
    public string? NationalRequirements { get; set; }

    /// <summary>
    ///     Was identity check done
    /// </summary>
    [JsonPropertyName("identityCheckDone")]
    public bool? IdentityCheckDone { get; set; }

    /// <summary>
    ///     Type of identity check performed
    /// </summary>
    [JsonPropertyName("identityCheckType")]
    public string? IdentityCheckType { get; set; }

    /// <summary>
    ///     Result of identity check
    /// </summary>
    [JsonPropertyName("identityCheckResult")]
    public string? IdentityCheckResult { get; set; }

    /// <summary>
    ///     What was the reason for skipping identity check
    /// </summary>
    [JsonPropertyName("identityCheckNotDoneReason")]
    public string? IdentityCheckNotDoneReason { get; set; }

    /// <summary>
    ///     Was physical check done
    /// </summary>
    [JsonPropertyName("physicalCheckDone")]
    public bool? PhysicalCheckDone { get; set; }

    /// <summary>
    ///     Result of physical check
    /// </summary>
    [JsonPropertyName("physicalCheckResult")]
    public string? PhysicalCheckResult { get; set; }

    /// <summary>
    ///     What was the reason for skipping physical check
    /// </summary>
    [JsonPropertyName("physicalCheckNotDoneReason")]
    public string? PhysicalCheckNotDoneReason { get; set; }

    /// <summary>
    ///     Other reason to not do physical check
    /// </summary>
    [JsonPropertyName("physicalCheckOtherText")]
    public string? PhysicalCheckOtherText { get; set; }

    /// <summary>
    ///     Welfare check
    /// </summary>
    [JsonPropertyName("welfareCheck")]
    public string? WelfareCheck { get; set; }

    /// <summary>
    ///     Number of animals checked
    /// </summary>
    [JsonPropertyName("numberOfAnimalsChecked")]
    public int? NumberOfAnimalsChecked { get; set; }

    /// <summary>
    ///     Were laboratory tests done
    /// </summary>
    [JsonPropertyName("laboratoryCheckDone")]
    public bool? LaboratoryCheckDone { get; set; }

    /// <summary>
    ///     Result of laboratory tests
    /// </summary>
    [JsonPropertyName("laboratoryCheckResult")]
    public string? LaboratoryCheckResult { get; set; }
}
