#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CatchCertificateDetailsMapper
{
    public static IpaffsDataApi.CatchCertificateDetails Map(CatchCertificateDetails? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.CatchCertificateDetails();
        to.CatchCertificateId = from?.CatchCertificateId;
        to.CatchCertificateReference = from?.CatchCertificateReference;
        to.IssuedOn = from?.DateOfIssue;
        to.FlagState = from?.FlagState;
        to.Species = from?.Species;
        return to;
    }
}
