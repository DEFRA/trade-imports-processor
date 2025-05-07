using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AddressMapper
{
    public static IpaffsDataApi.Address Map(Address? from)
    {
        if (from is null)
        {
            return null!;
        }
        var to = new IpaffsDataApi.Address
        {
            Street = from.Street,
            City = from.City,
            Country = from.Country,
            PostalCode = from.PostalCode,
            AddressLine1 = from.AddressLine1,
            AddressLine2 = from.AddressLine2,
            AddressLine3 = from.AddressLine3,
            PostalZipCode = from.PostalZipCode,
            CountryIsoCode = from.CountryIsoCode,
            Email = from.Email,
            UkTelephone = from.UkTelephone,
            Telephone = from.Telephone,
            InternationalTelephone = InternationalTelephoneMapper.Map(from.InternationalTelephone),
        };

        return to;
    }
}
