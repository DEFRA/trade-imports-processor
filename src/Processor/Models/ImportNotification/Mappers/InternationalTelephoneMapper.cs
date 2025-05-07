using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InternationalTelephoneMapper
{
    public static IpaffsDataApi.InternationalTelephone Map(InternationalTelephone? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.InternationalTelephone
        {
            CountryCode = from.CountryCode,
            SubscriberNumber = from.SubscriberNumber,
        };

        return to;
    }
}
