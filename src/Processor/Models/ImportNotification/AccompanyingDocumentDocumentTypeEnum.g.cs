
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum AccompanyingDocumentDocumentTypeEnum
{

    [EnumMember(Value = "airWaybill")]
    AirWaybill,

    [EnumMember(Value = "billOfLading")]
    BillOfLading,

    [EnumMember(Value = "cargoManifest")]
    CargoManifest,

    [EnumMember(Value = "catchCertificate")]
    CatchCertificate,

    [EnumMember(Value = "commercialDocument")]
    CommercialDocument,

    [EnumMember(Value = "commercialInvoice")]
    CommercialInvoice,

    [EnumMember(Value = "conformityCertificate")]
    ConformityCertificate,

    [EnumMember(Value = "containerManifest")]
    ContainerManifest,

    [EnumMember(Value = "customsDeclaration")]
    CustomsDeclaration,

    [EnumMember(Value = "docom")]
    Docom,

    [EnumMember(Value = "healthCertificate")]
    HealthCertificate,

    [EnumMember(Value = "heatTreatmentCertificate")]
    HeatTreatmentCertificate,

    [EnumMember(Value = "importPermit")]
    ImportPermit,

    [EnumMember(Value = "inspectionCertificate")]
    InspectionCertificate,

    [EnumMember(Value = "itahc")]
    Itahc,

    [EnumMember(Value = "journeyLog")]
    JourneyLog,

    [EnumMember(Value = "laboratorySamplingResultsForAflatoxin")]
    LaboratorySamplingResultsForAflatoxin,

    [EnumMember(Value = "latestVeterinaryHealthCertificate")]
    LatestVeterinaryHealthCertificate,

    [EnumMember(Value = "letterOfAuthority")]
    LetterOfAuthority,

    [EnumMember(Value = "licenseOrAuthorisation")]
    LicenseOrAuthorisation,

    [EnumMember(Value = "mycotoxinCertification")]
    MycotoxinCertification,

    [EnumMember(Value = "originCertificate")]
    OriginCertificate,

    [EnumMember(Value = "other")]
    Other,

    [EnumMember(Value = "phytosanitaryCertificate")]
    PhytosanitaryCertificate,

    [EnumMember(Value = "processingStatement")]
    ProcessingStatement,

    [EnumMember(Value = "proofOfStorage")]
    ProofOfStorage,

    [EnumMember(Value = "railwayBill")]
    RailwayBill,

    [EnumMember(Value = "seaWaybill")]
    SeaWaybill,

    [EnumMember(Value = "veterinaryHealthCertificate")]
    VeterinaryHealthCertificate,

    [EnumMember(Value = "listOfIngredients")]
    ListOfIngredients,

    [EnumMember(Value = "packingList")]
    PackingList,

    [EnumMember(Value = "roadConsignmentNote")]
    RoadConsignmentNote,

}