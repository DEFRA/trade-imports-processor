#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CatchCertificateAttachmentMapper
{
    public static IpaffsDataApi.CatchCertificateAttachment Map(CatchCertificateAttachment? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.CatchCertificateAttachment();
        to.AttachmentId = from?.AttachmentId;
        to.NumberOfCatchCertificates = from?.NumberOfCatchCertificates;
        to.CatchCertificateDetails = from
            ?.CatchCertificateDetails?.Select(x => CatchCertificateDetailsMapper.Map(x))
            .ToArray();
        return to;
    }
}
