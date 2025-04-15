using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class Identifiers
{
    /// <summary>
    ///     Number used to identify which item the identifiers are related to
    /// </summary>
    [JsonPropertyName("speciesNumber")]
    public int? SpeciesNumber { get; set; }

    /// <summary>
    ///     List of identifiers and their keys
    /// </summary>
    [JsonPropertyName("data")]
    public IDictionary<string, string>? Data { get; set; }

    /// <summary>
    ///     Is the place of destination the permanent address?
    /// </summary>
    [JsonPropertyName("isPlaceOfDestinationThePermanentAddress")]
    public bool? IsPlaceOfDestinationThePermanentAddress { get; set; }

    /// <summary>
    ///     Permanent address of the species
    /// </summary>
    [JsonPropertyName("permanentAddress")]
    public EconomicOperator? PermanentAddress { get; set; }
}
