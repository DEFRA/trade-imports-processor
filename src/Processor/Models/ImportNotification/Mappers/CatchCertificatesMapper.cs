using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CatchCertificatesMapper
{
    public static IpaffsDataApi.CatchCertificates Map(CatchCertificates? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.CatchCertificates();
        to.CertificateNumber = from?.CertificateNumber;
        to.Weight = from?.Weight;
        return to;
    }
}
