using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Control part of notification
/// </summary>
public class PartThree
{
    /// <summary>
    ///     Control status enum
    /// </summary>
    [JsonPropertyName("controlStatus")]
    public string? ControlStatus { get; set; }

    /// <summary>
    ///     Control details
    /// </summary>
    [JsonPropertyName("control")]
    public Control? Control { get; set; }

    /// <summary>
    ///     Validation messages for Part 3 - Control
    /// </summary>
    [JsonPropertyName("consignmentValidation")]
    public ValidationMessageCode[]? ConsignmentValidations { get; set; }

    /// <summary>
    ///     Is the seal check required
    /// </summary>
    [JsonPropertyName("sealCheckRequired")]
    public bool? SealCheckRequired { get; set; }

    /// <summary>
    ///     Seal check details
    /// </summary>
    [JsonPropertyName("sealCheck")]
    public SealCheck? SealCheck { get; set; }

    /// <summary>
    ///     Seal check override details
    /// </summary>
    [JsonPropertyName("sealCheckOverride")]
    public InspectionOverride? SealCheckOverride { get; set; }
}
