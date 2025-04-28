using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InternationalTelephoneMapper
{
    public static IpaffsDataApi.InternationalTelephone Map(InternationalTelephone? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.InternationalTelephone();
        to.CountryCode = from.CountryCode;
        to.SubscriberNumber = from.SubscriberNumber;
        return to;
    }
}
