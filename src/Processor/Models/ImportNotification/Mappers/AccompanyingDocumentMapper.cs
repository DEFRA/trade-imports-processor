using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AccompanyingDocumentMapper
{
    public static IpaffsDataApi.AccompanyingDocument Map(AccompanyingDocument? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.AccompanyingDocument();
        to.DocumentType = AccompanyingDocumentDocumentTypeMapper.Map(from?.DocumentType);
        to.DocumentReference = from?.DocumentReference;
        to.DocumentIssuedOn = from?.DocumentIssueDate;
        to.AttachmentId = from?.AttachmentId;
        to.AttachmentFilename = from?.AttachmentFilename;
        to.AttachmentContentType = from?.AttachmentContentType;
        to.UploadUserId = from?.UploadUserId;
        to.UploadOrganisationId = from?.UploadOrganisationId;
        to.ExternalReference = ExternalReferenceMapper.Map(from?.ExternalReference);
        return to;
    }
}
