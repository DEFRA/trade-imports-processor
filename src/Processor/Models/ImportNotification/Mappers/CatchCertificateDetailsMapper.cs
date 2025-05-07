using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CatchCertificateDetailsMapper
{
    public static IpaffsDataApi.CatchCertificateDetails Map(CatchCertificateDetails? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.CatchCertificateDetails
        {
            CatchCertificateId = from.CatchCertificateId,
            CatchCertificateReference = from.CatchCertificateReference,
            IssuedOn = from.DateOfIssue,
            FlagState = from.FlagState,
            Species = from.Species,
        };

        return to;
    }
}
