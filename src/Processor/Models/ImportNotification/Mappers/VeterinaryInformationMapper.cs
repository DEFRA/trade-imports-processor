using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class VeterinaryInformationMapper
{
    public static IpaffsDataApi.VeterinaryInformation Map(VeterinaryInformation? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.VeterinaryInformation();
        to.EstablishmentsOfOriginExternalReference = ExternalReferenceMapper.Map(
            from.EstablishmentsOfOriginExternalReference
        );
        to.EstablishmentsOfOrigins = from
            .EstablishmentsOfOrigins?.Select(x => ApprovedEstablishmentMapper.Map(x))
            .ToArray();
        to.VeterinaryDocument = from.VeterinaryDocument;
        to.VeterinaryDocumentIssuedOn = from.VeterinaryDocumentIssueDate;
        to.AccompanyingDocumentNumbers = from.AccompanyingDocumentNumbers;
        to.AccompanyingDocuments = from.AccompanyingDocuments?.Select(x => AccompanyingDocumentMapper.Map(x)).ToArray();
        to.CatchCertificateAttachments = from
            .CatchCertificateAttachments?.Select(x => CatchCertificateAttachmentMapper.Map(x))
            .ToArray();
        to.IdentificationDetails = from
            .IdentificationDetails?.Select(x => IdentificationDetailsMapper.Map(x))
            .ToArray();
        return to;
    }
}
