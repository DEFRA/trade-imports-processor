using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class Commodities
{
    /// <summary>
    ///     Flag to record when the GMS declaration has been accepted
    /// </summary>
    [JsonPropertyName("gmsDeclarationAccepted")]
    public bool? GmsDeclarationAccepted { get; set; }

    /// <summary>
    ///     Flag to record whether the consigned country is in an ipaffs charge group
    /// </summary>
    [JsonPropertyName("consignedCountryInChargeGroup")]
    public bool? ConsignedCountryInChargeGroup { get; set; }

    /// <summary>
    ///     The total gross weight of the consignment.  It must be bigger than the total net weight of the commodities
    /// </summary>
    [JsonPropertyName("totalGrossWeight")]
    public double? TotalGrossWeight { get; set; }

    /// <summary>
    ///     The total net weight of the commodities within this consignment
    /// </summary>
    [JsonPropertyName("totalNetWeight")]
    public double? TotalNetWeight { get; set; }

    /// <summary>
    ///     The total gross volume of the commodities within this consignment
    /// </summary>
    [JsonPropertyName("totalGrossVolume")]
    public double? TotalGrossVolume { get; set; }

    /// <summary>
    ///     Unit used for specifying total gross volume of this consignment (litres or metres cubed)
    /// </summary>
    [JsonPropertyName("totalGrossVolumeUnit")]
    public string? TotalGrossVolumeUnit { get; set; }

    /// <summary>
    ///     The total number of packages within this consignment
    /// </summary>
    [JsonPropertyName("numberOfPackages")]
    public int? NumberOfPackages { get; set; }

    /// <summary>
    ///     Temperature (type) of commodity
    /// </summary>
    [JsonPropertyName("temperature")]
    public string? Temperature { get; set; }

    /// <summary>
    ///     The total number of animals within this consignment
    /// </summary>
    [JsonPropertyName("numberOfAnimals")]
    public int? NumberOfAnimals { get; set; }

    [JsonPropertyName("commodityComplement")]
    public CommodityComplement[]? CommodityComplements { get; set; }

    /// <summary>
    ///     Additional data for commodityComplement part containing such data as net weight
    /// </summary>
    [JsonPropertyName("complementParameterSet")]
    public ComplementParameterSet[]? ComplementParameterSets { get; set; }

    /// <summary>
    ///     Does consignment contain ablacted animals
    /// </summary>
    [JsonPropertyName("includeNonAblactedAnimals")]
    public bool? IncludeNonAblactedAnimals { get; set; }

    /// <summary>
    ///     Consignments country of origin
    /// </summary>
    [JsonPropertyName("countryOfOrigin")]
    public string? CountryOfOrigin { get; set; }

    /// <summary>
    ///     Flag to record whether country of origin is a temporary PoD country
    /// </summary>
    [JsonPropertyName("countryOfOriginIsPodCountry")]
    public bool? CountryOfOriginIsPodCountry { get; set; }

    /// <summary>
    ///     Flag to record whether country of origin is a low risk article 72 country
    /// </summary>
    [JsonPropertyName("isLowRiskArticle72Country")]
    public bool? IsLowRiskArticle72Country { get; set; }

    /// <summary>
    ///     Region of country
    /// </summary>
    [JsonPropertyName("regionOfOrigin")]
    public string? RegionOfOrigin { get; set; }

    /// <summary>
    ///     Country from where commodity was sent
    /// </summary>
    [JsonPropertyName("consignedCountry")]
    public string? ConsignedCountry { get; set; }

    /// <summary>
    ///     Certification of animals (Breeding, slaughter etc.)
    /// </summary>
    [JsonPropertyName("animalsCertifiedAs")]
    public string? AnimalsCertifiedAs { get; set; }

    /// <summary>
    ///     What the commodity is intended for
    /// </summary>
    [JsonPropertyName("commodityIntendedFor")]
    public CommoditiesCommodityIntendedFor? CommodityIntendedFor { get; set; }
}
