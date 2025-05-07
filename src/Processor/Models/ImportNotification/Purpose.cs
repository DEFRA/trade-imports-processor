using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Purpose of consignment details
/// </summary>
public class Purpose
{
    /// <summary>
    ///     Does consignment conforms to UK laws
    /// </summary>
    [JsonPropertyName("conformsToEU")]
    public bool? ConformsToEU { get; set; }

    /// <summary>
    ///     Detailed purpose of internal market purpose group
    /// </summary>
    [JsonPropertyName("internalMarketPurpose")]
    public string? InternalMarketPurpose { get; set; }

    /// <summary>
    ///     Country that consignment is transshipped through
    /// </summary>
    [JsonPropertyName("thirdCountryTranshipment")]
    public string? ThirdCountryTranshipment { get; set; }

    /// <summary>
    ///     Detailed purpose for non conforming purpose group
    /// </summary>
    [JsonPropertyName("forNonConforming")]
    public string? ForNonConforming { get; set; }

    /// <summary>
    ///     There are 3 types of registration number based on the purpose of consignment. Customs registration number, Free
    ///     zone registration number and Shipping supplier registration number.
    /// </summary>
    [JsonPropertyName("regNumber")]
    public string? RegNumber { get; set; }

    /// <summary>
    ///     Ship name
    /// </summary>
    [JsonPropertyName("shipName")]
    public string? ShipName { get; set; }

    /// <summary>
    ///     Destination Ship port
    /// </summary>
    [JsonPropertyName("shipPort")]
    public string? ShipPort { get; set; }

    /// <summary>
    ///     Exit Border Inspection Post
    /// </summary>
    [JsonPropertyName("exitBIP")]
    public string? ExitBip { get; set; }

    /// <summary>
    ///     Country to which consignment is transited
    /// </summary>
    [JsonPropertyName("thirdCountry")]
    public string? ThirdCountry { get; set; }

    /// <summary>
    ///     Countries that consignment is transited through
    /// </summary>
    [JsonPropertyName("transitThirdCountries")]
    public string[]? TransitThirdCountries { get; set; }

    /// <summary>
    ///     Specification of Import or admission purpose
    /// </summary>
    [JsonPropertyName("forImportOrAdmission")]
    public string? ForImportOrAdmission { get; set; }

    /// <summary>
    ///     Exit date when import or admission
    /// </summary>
    [JsonPropertyName("exitDate")]
    public DateOnly? ExitDate { get; set; }

    /// <summary>
    ///     Final Border Inspection Post
    /// </summary>
    [JsonPropertyName("finalBIP")]
    public string? FinalBip { get; set; }

    [JsonPropertyName("pointOfExit")]
    public string? PointOfExit { get; init; }

    /// <summary>
    ///     Purpose group of consignment (general purpose)
    /// </summary>
    [JsonPropertyName("purposeGroup")]
    public string? PurposeGroup { get; set; }

    /// <summary>
    ///     Estimated date at port of exit
    /// </summary>
    [JsonPropertyName("estimatedArrivalDateAtPortOfExit")]
    public DateOnly? EstimatedArrivalDateAtPortOfExit { get; set; }

    /// <summary>
    ///     Estimated time at port of exit
    /// </summary>
    [JsonPropertyName("estimatedArrivalTimeAtPortOfExit")]
    public TimeOnly? EstimatedArrivalTimeAtPortOfExit { get; set; }
}
