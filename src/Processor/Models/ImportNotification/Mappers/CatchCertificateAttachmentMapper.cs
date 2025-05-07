using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CatchCertificateAttachmentMapper
{
    public static IpaffsDataApi.CatchCertificateAttachment Map(CatchCertificateAttachment? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.CatchCertificateAttachment
        {
            AttachmentId = from.AttachmentId,
            NumberOfCatchCertificates = from.NumberOfCatchCertificates,
            CatchCertificateDetails = from
                ?.CatchCertificateDetails?.Select(x => CatchCertificateDetailsMapper.Map(x))
                .ToArray(),
        };

        return to;
    }
}
