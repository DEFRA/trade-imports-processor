#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Authority which performed control
/// </summary>
public partial class ControlAuthority
{
    /// <summary>
    /// Official veterinarian
    /// </summary>
    [JsonPropertyName("officialVeterinarian")]
    public OfficialVeterinarian? OfficialVeterinarian { get; set; }

    /// <summary>
    /// Customs reference number
    /// </summary>
    [JsonPropertyName("customsReferenceNo")]
    public string? CustomsReferenceNo { get; set; }

    /// <summary>
    /// Were containers resealed?
    /// </summary>
    [JsonPropertyName("containerResealed")]
    public bool? ContainerResealed { get; set; }

    /// <summary>
    /// When the containers are resealed they need new seal numbers
    /// </summary>
    [JsonPropertyName("newSealNumber")]
    public string? NewSealNumber { get; set; }

    /// <summary>
    /// Illegal, Unreported and Unregulated (IUU) fishing reference number
    /// </summary>
    [JsonPropertyName("iuuFishingReference")]
    public string? IuuFishingReference { get; set; }

    /// <summary>
    /// Was Illegal, Unreported and Unregulated (IUU) check required
    /// </summary>
    [JsonPropertyName("iuuCheckRequired")]
    public bool? IuuCheckRequired { get; set; }

    /// <summary>
    /// Result of Illegal, Unreported and Unregulated (IUU) check
    /// </summary>
    [JsonPropertyName("iuuOption")]
    public ControlAuthorityIuuOption? IuuOption { get; set; }
}
