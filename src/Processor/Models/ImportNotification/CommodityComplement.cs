using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Holder for additional parameters of a commodity
/// </summary>
public class CommodityComplement
{
    /// <summary>
    ///     UUID used to match commodityComplement to its complementParameter set. CHEDPP only
    /// </summary>
    [JsonPropertyName("uniqueComplementID")]
    public string? UniqueComplementId { get; set; }

    /// <summary>
    ///     Description of the commodity
    /// </summary>
    [JsonPropertyName("commodityDescription")]
    public string? CommodityDescription { get; set; }

    /// <summary>
    ///     The unique commodity ID
    /// </summary>
    [JsonPropertyName("commodityID")]
    public string? CommodityId { get; set; }

    /// <summary>
    ///     Identifier of complement chosen from speciesFamily,speciesClass and speciesType&#x27;. This is also used to link to
    ///     the complementParameterSet
    /// </summary>
    [JsonPropertyName("complementID")]
    public int? ComplementId { get; set; }

    /// <summary>
    ///     Represents the lowest granularity - either type, class, family or species name - for the commodity selected.  This
    ///     is also used to drive behaviour for EU Import journeys
    /// </summary>
    [JsonPropertyName("complementName")]
    public string? ComplementName { get; set; }

    /// <summary>
    ///     EPPO Code related to plant commodities and wood packaging
    /// </summary>
    [JsonPropertyName("eppoCode")]
    public string? EppoCode { get; set; }

    /// <summary>
    ///     (Deprecated in IMTA-11832) Is this commodity wood packaging?
    /// </summary>
    [JsonPropertyName("isWoodPackaging")]
    public bool? IsWoodPackaging { get; set; }

    /// <summary>
    ///     The species ID of the commodity that is imported. Not every commodity has a species ID. This is also used to link
    ///     to the complementParameterSet. The species ID can change over time
    /// </summary>
    [JsonPropertyName("speciesID")]
    public string? SpeciesId { get; set; }

    /// <summary>
    ///     Species name
    /// </summary>
    [JsonPropertyName("speciesName")]
    public string? SpeciesName { get; set; }

    /// <summary>
    ///     Species nomination
    /// </summary>
    [JsonPropertyName("speciesNomination")]
    public string? SpeciesNomination { get; set; }

    /// <summary>
    ///     Species type name
    /// </summary>
    [JsonPropertyName("speciesTypeName")]
    public string? SpeciesTypeName { get; set; }

    /// <summary>
    ///     Species type identifier of commodity
    /// </summary>
    [JsonPropertyName("speciesType")]
    public string? SpeciesType { get; set; }

    /// <summary>
    ///     Species class name
    /// </summary>
    [JsonPropertyName("speciesClassName")]
    public string? SpeciesClassName { get; set; }

    /// <summary>
    ///     Species class identifier of commodity
    /// </summary>
    [JsonPropertyName("speciesClass")]
    public string? SpeciesClass { get; set; }

    /// <summary>
    ///     Species family name of commodity
    /// </summary>
    [JsonPropertyName("speciesFamilyName")]
    public string? SpeciesFamilyName { get; set; }

    /// <summary>
    ///     Species family identifier of commodity
    /// </summary>
    [JsonPropertyName("speciesFamily")]
    public string? SpeciesFamily { get; set; }

    /// <summary>
    ///     Species common name of commodity for IMP notification simple commodity selection
    /// </summary>
    [JsonPropertyName("speciesCommonName")]
    public string? SpeciesCommonName { get; set; }

    /// <summary>
    ///     Has commodity been matched with corresponding CDS declaration
    /// </summary>
    [JsonPropertyName("isCdsMatched")]
    public bool? IsCdsMatched { get; set; }

    [JsonPropertyName("additionalData")]
    public IDictionary<string, object>? AdditionalData { get; set; }

    [JsonPropertyName("riskAssesment")]
    public CommodityRiskResult? RiskAssesment { get; set; }

    [JsonPropertyName("checks")]
    public InspectionCheck[]? Checks { get; set; }
}
