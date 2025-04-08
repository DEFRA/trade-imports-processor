#nullable enable



using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PostalAddressMapper
{
    public static IpaffsDataApi.PostalAddress Map(PostalAddress? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.PostalAddress();
        to.AddressLine1 = from?.AddressLine1;
        to.AddressLine2 = from?.AddressLine2;
        to.AddressLine3 = from?.AddressLine3;
        to.AddressLine4 = from?.AddressLine4;
        to.County = from?.County;
        to.CityOrTown = from?.CityOrTown;
        to.PostalCode = from?.PostalCode;
        return to;
    }
}
