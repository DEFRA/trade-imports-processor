using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AccompanyingDocumentMapper
{
    public static IpaffsDataApi.AccompanyingDocument Map(AccompanyingDocument? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.AccompanyingDocument
        {
            DocumentType = from.DocumentType,
            DocumentReference = from.DocumentReference,
            DocumentIssuedOn = from.DocumentIssueDate,
            AttachmentId = from.AttachmentId,
            AttachmentFilename = from.AttachmentFilename,
            AttachmentContentType = from.AttachmentContentType,
            UploadUserId = from.UploadUserId,
            UploadOrganisationId = from.UploadOrganisationId,
            ExternalReference = ExternalReferenceMapper.Map(from.ExternalReference),
        };

        return to;
    }
}
