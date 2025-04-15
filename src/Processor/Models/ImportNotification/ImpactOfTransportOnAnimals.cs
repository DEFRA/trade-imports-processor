using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Impact of transport on animals
/// </summary>
public class ImpactOfTransportOnAnimals
{
    /// <summary>
    ///     Number of dead animals specified by units
    /// </summary>
    [JsonPropertyName("numberOfDeadAnimals")]
    public int? NumberOfDeadAnimals { get; set; }

    /// <summary>
    ///     Unit used for specifying number of dead animals (percent or units)
    /// </summary>
    [JsonPropertyName("numberOfDeadAnimalsUnit")]
    public string? NumberOfDeadAnimalsUnit { get; set; }

    /// <summary>
    ///     Number of unfit animals
    /// </summary>
    [JsonPropertyName("numberOfUnfitAnimals")]
    public int? NumberOfUnfitAnimals { get; set; }

    /// <summary>
    ///     Unit used for specifying number of unfit animals (percent or units)
    /// </summary>
    [JsonPropertyName("numberOfUnfitAnimalsUnit")]
    public string? NumberOfUnfitAnimalsUnit { get; set; }

    /// <summary>
    ///     Number of births or abortions (unit)
    /// </summary>
    [JsonPropertyName("numberOfBirthOrAbortion")]
    public int? NumberOfBirthOrAbortion { get; set; }
}
