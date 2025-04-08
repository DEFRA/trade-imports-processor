#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Part 2 of notification - Decision at inspection
/// </summary>
public partial class PartTwo
{
    /// <summary>
    /// Decision on the consignment
    /// </summary>
    [JsonPropertyName("decision")]
    public Decision? Decision { get; set; }

    /// <summary>
    /// Consignment check
    /// </summary>
    [JsonPropertyName("consignmentCheck")]
    public ConsignmentCheck? ConsignmentCheck { get; set; }

    /// <summary>
    /// Checks of impact of transport on animals
    /// </summary>
    [JsonPropertyName("impactOfTransportOnAnimals")]
    public ImpactOfTransportOnAnimals? ImpactOfTransportOnAnimals { get; set; }

    /// <summary>
    /// Are laboratory tests required
    /// </summary>
    [JsonPropertyName("laboratoryTestsRequired")]
    public bool? LaboratoryTestsRequired { get; set; }

    /// <summary>
    /// Laboratory tests information details
    /// </summary>
    [JsonPropertyName("laboratoryTests")]
    public LaboratoryTests? LaboratoryTests { get; set; }

    /// <summary>
    /// Are the containers resealed
    /// </summary>
    [JsonPropertyName("resealedContainersIncluded")]
    public bool? ResealedContainersIncluded { get; set; }

    /// <summary>
    /// (Deprecated - To be removed as part of IMTA-6256) Resealed containers information details
    /// </summary>
    [JsonPropertyName("resealedContainers")]
    public string[]? ResealedContainers { get; set; }

    /// <summary>
    /// Resealed containers information details
    /// </summary>
    [JsonPropertyName("resealedContainersMapping")]
    public SealContainer[]? ResealedContainersMappings { get; set; }

    /// <summary>
    /// Control Authority information details
    /// </summary>
    [JsonPropertyName("controlAuthority")]
    public ControlAuthority? ControlAuthority { get; set; }

    /// <summary>
    /// Controlled destination
    /// </summary>
    [JsonPropertyName("controlledDestination")]
    public EconomicOperator? ControlledDestination { get; set; }

    /// <summary>
    /// Local reference number at BIP
    /// </summary>
    [JsonPropertyName("bipLocalReferenceNumber")]
    public string? BipLocalReferenceNumber { get; set; }

    /// <summary>
    /// Part 2 - Sometimes other user can sign decision on behalf of another user
    /// </summary>
    [JsonPropertyName("signedOnBehalfOf")]
    public string? SignedOnBehalfOf { get; set; }

    /// <summary>
    /// Onward transportation
    /// </summary>
    [JsonPropertyName("onwardTransportation")]
    public string? OnwardTransportation { get; set; }

    /// <summary>
    /// Validation messages for Part 2 - Decision
    /// </summary>
    [JsonPropertyName("consignmentValidation")]
    public ValidationMessageCode[]? ConsignmentValidations { get; set; }

    /// <summary>
    /// User entered date when the checks were completed
    /// </summary>
    [JsonPropertyName("checkDate")]
    public DateTime? CheckDate { get; set; }

    /// <summary>
    /// Accompanying documents
    /// </summary>
    [JsonPropertyName("accompanyingDocuments")]
    public AccompanyingDocument[]? AccompanyingDocuments { get; set; }

    [JsonPropertyName("commodityChecks")]
    public CommodityChecks[]? CommodityChecks { get; set; }

    /// <summary>
    /// Have the PHSI regulated commodities been auto cleared?
    /// </summary>
    [JsonPropertyName("phsiAutoCleared")]
    public bool? PhsiAutoCleared { get; set; }

    /// <summary>
    /// Have the HMI regulated commodities been auto cleared?
    /// </summary>
    [JsonPropertyName("hmiAutoCleared")]
    public bool? HmiAutoCleared { get; set; }

    /// <summary>
    /// Inspection required
    /// </summary>
    [JsonPropertyName("inspectionRequired")]
    public InspectionRequired? InspectionRequired { get; set; }

    /// <summary>
    /// Details about the manual inspection override
    /// </summary>
    [JsonPropertyName("inspectionOverride")]
    public InspectionOverride? InspectionOverride { get; set; }

    /// <summary>
    /// Date of autoclearance
    /// </summary>
    [JsonPropertyName("autoClearedDateTime")]
    public DateTime? AutoClearedDateTime { get; set; }
}
