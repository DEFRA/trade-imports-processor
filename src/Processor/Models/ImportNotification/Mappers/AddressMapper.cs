using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AddressMapper
{
    public static IpaffsDataApi.Address Map(Address? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.Address();
        to.Street = from.Street;
        to.City = from.City;
        to.Country = from.Country;
        to.PostalCode = from.PostalCode;
        to.AddressLine1 = from.AddressLine1;
        to.AddressLine2 = from.AddressLine2;
        to.AddressLine3 = from.AddressLine3;
        to.PostalZipCode = from.PostalZipCode;
        to.CountryIsoCode = from.CountryIsoCode;
        to.Email = from.Email;
        to.UkTelephone = from.UkTelephone;
        to.Telephone = from.Telephone;
        to.InternationalTelephone = InternationalTelephoneMapper.Map(from.InternationalTelephone);
        return to;
    }
}
