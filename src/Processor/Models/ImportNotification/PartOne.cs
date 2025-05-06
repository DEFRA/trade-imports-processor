using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class PartOne
{
    /// <summary>
    ///     Used to indicate what type of EU Import the notification is - Live Animals, Product Of Animal Origin or High Risk
    ///     Food Not Of Animal Origin
    /// </summary>
    [JsonPropertyName("typeOfImp")]
    public string? TypeOfImp { get; set; }

    /// <summary>
    ///     The individual who has submitted the notification
    /// </summary>
    [JsonPropertyName("personResponsible")]
    public Party? PersonResponsible { get; set; }

    /// <summary>
    ///     Customs reference number
    /// </summary>
    [JsonPropertyName("customsReferenceNumber")]
    public string? CustomsReferenceNumber { get; set; }

    /// <summary>
    ///     (Deprecated in IMTA-11832) Does the consignment contain wood packaging?
    /// </summary>
    [JsonPropertyName("containsWoodPackaging")]
    public bool? ContainsWoodPackaging { get; set; }

    /// <summary>
    ///     Has the consignment arrived at the BCP?
    /// </summary>
    [JsonPropertyName("consignmentArrived")]
    public bool? ConsignmentArrived { get; set; }

    /// <summary>
    ///     Person or Company that sends shipment
    /// </summary>
    [JsonPropertyName("consignor")]
    public EconomicOperator? Consignor { get; set; }

    /// <summary>
    ///     Person or Company that sends shipment
    /// </summary>
    [JsonPropertyName("consignorTwo")]
    public EconomicOperator? ConsignorTwo { get; set; }

    /// <summary>
    ///     Person or Company that packs the shipment
    /// </summary>
    [JsonPropertyName("packer")]
    public EconomicOperator? Packer { get; set; }

    /// <summary>
    ///     Person or Company that receives shipment
    /// </summary>
    [JsonPropertyName("consignee")]
    public EconomicOperator? Consignee { get; set; }

    /// <summary>
    ///     Person or Company that is importing the consignment
    /// </summary>
    [JsonPropertyName("importer")]
    public EconomicOperator? Importer { get; set; }

    /// <summary>
    ///     Where the shipment is to be sent? For IMP minimum 48 hour accommodation/holding location.
    /// </summary>
    [JsonPropertyName("placeOfDestination")]
    public EconomicOperator? PlaceOfDestination { get; set; }

    /// <summary>
    ///     A temporary place of destination for plants
    /// </summary>
    [JsonPropertyName("pod")]
    public EconomicOperator? Pod { get; set; }

    /// <summary>
    ///     Place in which the animals or products originate
    /// </summary>
    [JsonPropertyName("placeOfOriginHarvest")]
    public EconomicOperator? PlaceOfOriginHarvest { get; set; }

    /// <summary>
    ///     List of additional permanent addresses
    /// </summary>
    [JsonPropertyName("additionalPermanentAddresses")]
    public EconomicOperator[]? AdditionalPermanentAddresses { get; set; }

    /// <summary>
    ///     Charity Parish Holding number
    /// </summary>
    [JsonPropertyName("cphNumber")]
    public string? CphNumber { get; set; }

    /// <summary>
    ///     Is the importer importing from a charity?
    /// </summary>
    [JsonPropertyName("importingFromCharity")]
    public bool? ImportingFromCharity { get; set; }

    /// <summary>
    ///     Is the place of destination the permanent address?
    /// </summary>
    [JsonPropertyName("isPlaceOfDestinationThePermanentAddress")]
    public bool? IsPlaceOfDestinationThePermanentAddress { get; set; }

    /// <summary>
    ///     Is this catch certificate required?
    /// </summary>
    [JsonPropertyName("isCatchCertificateRequired")]
    public bool? IsCatchCertificateRequired { get; set; }

    /// <summary>
    ///     Is GVMS route?
    /// </summary>
    [JsonPropertyName("isGVMSRoute")]
    public bool? IsGvmsRoute { get; set; }

    [JsonPropertyName("commodities")]
    public Commodities? Commodities { get; set; }

    /// <summary>
    ///     Purpose of consignment details
    /// </summary>
    [JsonPropertyName("purpose")]
    public Purpose? Purpose { get; set; }

    /// <summary>
    ///     Either a Border-Inspection-Post or Designated-Point-Of-Entry, e.g. GBFXT1
    /// </summary>
    [JsonPropertyName("pointOfEntry")]
    public string? PointOfEntry { get; set; }

    /// <summary>
    ///     A control point at the point of entry
    /// </summary>
    [JsonPropertyName("pointOfEntryControlPoint")]
    public string? PointOfEntryControlPoint { get; set; }

    /// <summary>
    ///     Date when consignment arrives
    /// </summary>
    [JsonPropertyName("arrivalDate")]
    public DateOnly? ArrivalDate { get; set; }

    /// <summary>
    ///     Time (HH:MM) when consignment arrives
    /// </summary>
    [JsonPropertyName("arrivalTime")]
    public TimeOnly? ArrivalTime { get; set; }

    /// <summary>
    ///     How consignment is transported after BIP
    /// </summary>
    [JsonPropertyName("meansOfTransport")]
    public MeansOfTransport? MeansOfTransport { get; set; }

    /// <summary>
    ///     Transporter of consignment details
    /// </summary>
    [JsonPropertyName("transporter")]
    public EconomicOperator? Transporter { get; set; }

    /// <summary>
    ///     Are transporter details required for this consignment
    /// </summary>
    [JsonPropertyName("transporterDetailsRequired")]
    public bool? TransporterDetailsRequired { get; set; }

    /// <summary>
    ///     Transport to BIP
    /// </summary>
    [JsonPropertyName("meansOfTransportFromEntryPoint")]
    public MeansOfTransport? MeansOfTransportFromEntryPoint { get; set; }

    /// <summary>
    ///     Date of consignment departure
    /// </summary>
    [JsonPropertyName("departureDate")]
    public DateOnly? DepartureDate { get; set; }

    /// <summary>
    ///     Time (HH:MM) of consignment departure
    /// </summary>
    [JsonPropertyName("departureTime")]
    public TimeOnly? DepartureTime { get; set; }

    /// <summary>
    ///     Estimated journey time in minutes to point of entry
    /// </summary>
    [JsonPropertyName("estimatedJourneyTimeInMinutes")]
    public double? EstimatedJourneyTimeInMinutes { get; set; }

    /// <summary>
    ///     (Deprecated in IMTA-12139) Person who is responsible for transport
    /// </summary>
    [JsonPropertyName("responsibleForTransport")]
    public string? ResponsibleForTransport { get; set; }

    /// <summary>
    ///     Part 1 - Holds the information related to veterinary checks and details
    /// </summary>
    [JsonPropertyName("veterinaryInformation")]
    public VeterinaryInformation? VeterinaryInformation { get; set; }

    /// <summary>
    ///     Reference number added by the importer
    /// </summary>
    [JsonPropertyName("importerLocalReferenceNumber")]
    public string? ImporterLocalReferenceNumber { get; set; }

    /// <summary>
    ///     Contains countries and transfer points that consignment is going through
    /// </summary>
    [JsonPropertyName("route")]
    public Route? Route { get; set; }

    /// <summary>
    ///     Array that contains pair of seal number and container number
    /// </summary>
    [JsonPropertyName("sealsContainers")]
    public SealContainer[]? SealsContainers { get; set; }

    /// <summary>
    ///     Date and time when the notification was submitted
    /// </summary>
    [JsonPropertyName("submissionDate")]
    public DateTime? SubmittedOn { get; set; }

    /// <summary>
    ///     Information about user who submitted notification
    /// </summary>
    [JsonPropertyName("submittedBy")]
    public UserInformation? SubmittedBy { get; set; }

    /// <summary>
    ///     Validation messages for whole notification
    /// </summary>
    [JsonPropertyName("consignmentValidation")]
    public ValidationMessageCode[]? ConsignmentValidations { get; set; }

    /// <summary>
    ///     Was complex commodity selected. Indicating if importer provided commodity code.
    /// </summary>
    [JsonPropertyName("complexCommoditySelected")]
    public bool? ComplexCommoditySelected { get; set; }

    /// <summary>
    ///     Entry port for EU Import notification.
    /// </summary>
    [JsonPropertyName("portOfEntry")]
    public string? PortOfEntry { get; set; }

    /// <summary>
    ///     Exit Port for EU Import Notification.
    /// </summary>
    [JsonPropertyName("portOfExit")]
    public string? PortOfExit { get; set; }

    /// <summary>
    ///     Date of Port Exit for EU Import Notification.
    /// </summary>
    [JsonPropertyName("portOfExitDate")]
    [UnknownTimeZoneDateTimeJsonConverter(nameof(PortOfExitDate))]
    public DateTime? PortOfExitDate { get; set; }

    /// <summary>
    ///     Person to be contacted if there is an issue with the consignment
    /// </summary>
    [JsonPropertyName("contactDetails")]
    public ContactDetails? ContactDetails { get; set; }

    /// <summary>
    ///     List of nominated contacts to receive text and email notifications
    /// </summary>
    [JsonPropertyName("nominatedContacts")]
    public NominatedContact[]? NominatedContacts { get; set; }

    /// <summary>
    ///     Original estimated date time of arrival
    /// </summary>
    [JsonPropertyName("originalEstimatedDateTime")]
    public DateTime? OriginalEstimatedOn { get; set; }

    [JsonPropertyName("billingInformation")]
    public BillingInformation? BillingInformation { get; set; }

    /// <summary>
    ///     Indicates whether CUC applies to the notification
    /// </summary>
    [JsonPropertyName("isChargeable")]
    public bool? IsChargeable { get; set; }

    /// <summary>
    ///     Indicates whether CUC previously applied to the notification
    /// </summary>
    [JsonPropertyName("wasChargeable")]
    public bool? WasChargeable { get; set; }

    [JsonPropertyName("commonUserCharge")]
    public CommonUserCharge? CommonUserCharge { get; set; }

    /// <summary>
    ///     When the NCTS MRN will be added for the Common Transit Convention (CTC)
    /// </summary>
    [JsonPropertyName("provideCtcMrn")]
    public string? ProvideCtcMrn { get; set; }
}
