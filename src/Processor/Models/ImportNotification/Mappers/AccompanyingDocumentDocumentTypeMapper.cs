using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AccompanyingDocumentDocumentTypeMapper
{
    public static IpaffsDataApi.AccompanyingDocumentDocumentType? Map(AccompanyingDocumentDocumentType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            AccompanyingDocumentDocumentType.AirWaybill => IpaffsDataApi.AccompanyingDocumentDocumentType.AirWaybill,
            AccompanyingDocumentDocumentType.BillOfLading => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .BillOfLading,
            AccompanyingDocumentDocumentType.CargoManifest => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .CargoManifest,
            AccompanyingDocumentDocumentType.CatchCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .CatchCertificate,
            AccompanyingDocumentDocumentType.CommercialDocument => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .CommercialDocument,
            AccompanyingDocumentDocumentType.CommercialInvoice => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .CommercialInvoice,
            AccompanyingDocumentDocumentType.ConformityCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ConformityCertificate,
            AccompanyingDocumentDocumentType.ContainerManifest => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ContainerManifest,
            AccompanyingDocumentDocumentType.CustomsDeclaration => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .CustomsDeclaration,
            AccompanyingDocumentDocumentType.Docom => IpaffsDataApi.AccompanyingDocumentDocumentType.Docom,
            AccompanyingDocumentDocumentType.HealthCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .HealthCertificate,
            AccompanyingDocumentDocumentType.HeatTreatmentCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .HeatTreatmentCertificate,
            AccompanyingDocumentDocumentType.ImportPermit => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ImportPermit,
            AccompanyingDocumentDocumentType.InspectionCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .InspectionCertificate,
            AccompanyingDocumentDocumentType.Itahc => IpaffsDataApi.AccompanyingDocumentDocumentType.Itahc,
            AccompanyingDocumentDocumentType.JourneyLog => IpaffsDataApi.AccompanyingDocumentDocumentType.JourneyLog,
            AccompanyingDocumentDocumentType.LaboratorySamplingResultsForAflatoxin => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .LaboratorySamplingResultsForAflatoxin,
            AccompanyingDocumentDocumentType.LatestVeterinaryHealthCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .LatestVeterinaryHealthCertificate,
            AccompanyingDocumentDocumentType.LetterOfAuthority => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .LetterOfAuthority,
            AccompanyingDocumentDocumentType.LicenseOrAuthorisation => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .LicenseOrAuthorisation,
            AccompanyingDocumentDocumentType.MycotoxinCertification => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .MycotoxinCertification,
            AccompanyingDocumentDocumentType.OriginCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .OriginCertificate,
            AccompanyingDocumentDocumentType.Other => IpaffsDataApi.AccompanyingDocumentDocumentType.Other,
            AccompanyingDocumentDocumentType.PhytosanitaryCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .PhytosanitaryCertificate,
            AccompanyingDocumentDocumentType.ProcessingStatement => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ProcessingStatement,
            AccompanyingDocumentDocumentType.ProofOfStorage => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ProofOfStorage,
            AccompanyingDocumentDocumentType.RailwayBill => IpaffsDataApi.AccompanyingDocumentDocumentType.RailwayBill,
            AccompanyingDocumentDocumentType.SeaWaybill => IpaffsDataApi.AccompanyingDocumentDocumentType.SeaWaybill,
            AccompanyingDocumentDocumentType.VeterinaryHealthCertificate => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .VeterinaryHealthCertificate,
            AccompanyingDocumentDocumentType.ListOfIngredients => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .ListOfIngredients,
            AccompanyingDocumentDocumentType.PackingList => IpaffsDataApi.AccompanyingDocumentDocumentType.PackingList,
            AccompanyingDocumentDocumentType.RoadConsignmentNote => IpaffsDataApi
                .AccompanyingDocumentDocumentType
                .RoadConsignmentNote,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
