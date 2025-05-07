using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class VeterinaryInformationMapper
{
    public static IpaffsDataApi.VeterinaryInformation Map(VeterinaryInformation? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.VeterinaryInformation
        {
            EstablishmentsOfOriginExternalReference = ExternalReferenceMapper.Map(
                from.EstablishmentsOfOriginExternalReference
            ),
            EstablishmentsOfOrigins = from
                .EstablishmentsOfOrigins?.Select(x => ApprovedEstablishmentMapper.Map(x))
                .ToArray(),
            VeterinaryDocument = from.VeterinaryDocument,
            VeterinaryDocumentIssuedOn = from.VeterinaryDocumentIssueDate,
            AccompanyingDocumentNumbers = from.AccompanyingDocumentNumbers,
            AccompanyingDocuments = from
                .AccompanyingDocuments?.Select(x => AccompanyingDocumentMapper.Map(x))
                .ToArray(),
            CatchCertificateAttachments = from
                .CatchCertificateAttachments?.Select(x => CatchCertificateAttachmentMapper.Map(x))
                .ToArray(),
            IdentificationDetails = from
                .IdentificationDetails?.Select(x => IdentificationDetailsMapper.Map(x))
                .ToArray(),
        };

        return to;
    }
}
