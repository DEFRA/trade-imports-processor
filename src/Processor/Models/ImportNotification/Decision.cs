using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Decision if the consignment can be imported
/// </summary>
public class Decision
{
    /// <summary>
    ///     Is consignment acceptable or not
    /// </summary>
    [JsonPropertyName("consignmentAcceptable")]
    public bool? ConsignmentAcceptable { get; set; }

    /// <summary>
    ///     Filled if consignmentAcceptable is set to false
    /// </summary>
    [JsonPropertyName("notAcceptableAction")]
    public string? NotAcceptableAction { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to destruction
    /// </summary>
    [JsonPropertyName("notAcceptableActionDestructionReason")]
    public string? NotAcceptableActionDestructionReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to entry refusal
    /// </summary>
    [JsonPropertyName("notAcceptableActionEntryRefusalReason")]
    public string? NotAcceptableActionEntryRefusalReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to quarantine imposed
    /// </summary>
    [JsonPropertyName("notAcceptableActionQuarantineImposedReason")]
    public string? NotAcceptableActionQuarantineImposedReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to special treatment
    /// </summary>
    [JsonPropertyName("notAcceptableActionSpecialTreatmentReason")]
    public string? NotAcceptableActionSpecialTreatmentReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to industrial processing
    /// </summary>
    [JsonPropertyName("notAcceptableActionIndustrialProcessingReason")]
    public string? NotAcceptableActionIndustrialProcessingReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to re-dispatch
    /// </summary>
    [JsonPropertyName("notAcceptableActionReDispatchReason")]
    public string? NotAcceptableActionReDispatchReason { get; set; }

    /// <summary>
    ///     Filled if not acceptable action is set to use for other purposes
    /// </summary>
    [JsonPropertyName("notAcceptableActionUseForOtherPurposesReason")]
    public string? NotAcceptableActionUseForOtherPurposesReason { get; set; }

    /// <summary>
    ///     Filled when notAcceptableAction is equal to destruction
    /// </summary>
    [JsonPropertyName("notAcceptableDestructionReason")]
    public string? NotAcceptableDestructionReason { get; set; }

    /// <summary>
    ///     Filled when notAcceptableAction is equal to other
    /// </summary>
    [JsonPropertyName("notAcceptableActionOtherReason")]
    public string? NotAcceptableActionOtherReason { get; set; }

    /// <summary>
    ///     Filled when consignmentAcceptable is set to false
    /// </summary>
    [JsonPropertyName("notAcceptableActionByDate")]
    public DateOnly? NotAcceptableActionByDate { get; set; }

    /// <summary>
    ///     List of details for individual chedpp not acceptable reasons
    /// </summary>
    [JsonPropertyName("chedppNotAcceptableReasons")]
    public ChedppNotAcceptableReason[]? ChedppNotAcceptableReasons { get; set; }

    /// <summary>
    ///     If the consignment was not accepted what was the reason
    /// </summary>
    [JsonPropertyName("notAcceptableReasons")]
    public string[]? NotAcceptableReasons { get; set; }

    /// <summary>
    ///     2 digits ISO code of country (not acceptable country can be empty)
    /// </summary>
    [JsonPropertyName("notAcceptableCountry")]
    public string? NotAcceptableCountry { get; set; }

    /// <summary>
    ///     Filled if consignmentAcceptable is set to false
    /// </summary>
    [JsonPropertyName("notAcceptableEstablishment")]
    public string? NotAcceptableEstablishment { get; set; }

    /// <summary>
    ///     Filled if consignmentAcceptable is set to false
    /// </summary>
    [JsonPropertyName("notAcceptableOtherReason")]
    public string? NotAcceptableOtherReason { get; set; }

    /// <summary>
    ///     Details of controlled destinations
    /// </summary>
    [JsonPropertyName("detailsOfControlledDestinations")]
    public Party? DetailsOfControlledDestinations { get; set; }

    /// <summary>
    ///     Filled if consignment is set to acceptable and decision type is Specific Warehouse
    /// </summary>
    [JsonPropertyName("specificWarehouseNonConformingConsignment")]
    public string? SpecificWarehouseNonConformingConsignment { get; set; }

    /// <summary>
    ///     Deadline when consignment has to leave borders
    /// </summary>
    [JsonPropertyName("temporaryDeadline")]
    public string? TemporaryDeadline { get; set; }

    /// <summary>
    ///     Detailed decision for consignment
    /// </summary>
    [JsonPropertyName("decision")]
    public string? DecisionEnum { get; set; }

    /// <summary>
    ///     Decision over purpose of free circulation in country
    /// </summary>
    [JsonPropertyName("freeCirculationPurpose")]
    public string? FreeCirculationPurpose { get; set; }

    /// <summary>
    ///     Decision over purpose of definitive import
    /// </summary>
    [JsonPropertyName("definitiveImportPurpose")]
    public string? DefinitiveImportPurpose { get; set; }

    /// <summary>
    ///     Decision channeled option based on (article8, article15)
    /// </summary>
    [JsonPropertyName("ifChanneledOption")]
    public string? IfChanneledOption { get; set; }

    /// <summary>
    ///     Custom warehouse registered number
    /// </summary>
    [JsonPropertyName("customWarehouseRegisteredNumber")]
    public string? CustomWarehouseRegisteredNumber { get; set; }

    /// <summary>
    ///     Free warehouse registered number
    /// </summary>
    [JsonPropertyName("freeWarehouseRegisteredNumber")]
    public string? FreeWarehouseRegisteredNumber { get; set; }

    /// <summary>
    ///     Ship name
    /// </summary>
    [JsonPropertyName("shipName")]
    public string? ShipName { get; set; }

    /// <summary>
    ///     Port of exit
    /// </summary>
    [JsonPropertyName("shipPortOfExit")]
    public string? ShipPortOfExit { get; set; }

    /// <summary>
    ///     Ship supplier registered number
    /// </summary>
    [JsonPropertyName("shipSupplierRegisteredNumber")]
    public string? ShipSupplierRegisteredNumber { get; set; }

    /// <summary>
    ///     Transhipment BIP
    /// </summary>
    [JsonPropertyName("transhipmentBip")]
    public string? TranshipmentBip { get; set; }

    /// <summary>
    ///     Transhipment third country
    /// </summary>
    [JsonPropertyName("transhipmentThirdCountry")]
    public string? TranshipmentThirdCountry { get; set; }

    /// <summary>
    ///     Transit exit BIP
    /// </summary>
    [JsonPropertyName("transitExitBip")]
    public string? TransitExitBip { get; set; }

    /// <summary>
    ///     Transit third country
    /// </summary>
    [JsonPropertyName("transitThirdCountry")]
    public string? TransitThirdCountry { get; set; }

    /// <summary>
    ///     Transit destination third country
    /// </summary>
    [JsonPropertyName("transitDestinationThirdCountry")]
    public string? TransitDestinationThirdCountry { get; set; }

    /// <summary>
    ///     Temporary exit BIP
    /// </summary>
    [JsonPropertyName("temporaryExitBip")]
    public string? TemporaryExitBip { get; set; }

    /// <summary>
    ///     Horse re-entry
    /// </summary>
    [JsonPropertyName("horseReentry")]
    public string? HorseReentry { get; set; }

    /// <summary>
    ///     Is it transshipped to EU or third country (values EU / country name)
    /// </summary>
    [JsonPropertyName("transhipmentEuOrThirdCountry")]
    public string? TranshipmentEuOrThirdCountry { get; set; }
}
