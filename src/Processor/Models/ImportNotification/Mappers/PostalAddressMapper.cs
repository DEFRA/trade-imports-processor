using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PostalAddressMapper
{
    public static IpaffsDataApi.PostalAddress Map(PostalAddress? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.PostalAddress
        {
            AddressLine1 = from.AddressLine1,
            AddressLine2 = from.AddressLine2,
            AddressLine3 = from.AddressLine3,
            AddressLine4 = from.AddressLine4,
            County = from.County,
            CityOrTown = from.CityOrTown,
            PostalCode = from.PostalCode,
        };

        return to;
    }
}
