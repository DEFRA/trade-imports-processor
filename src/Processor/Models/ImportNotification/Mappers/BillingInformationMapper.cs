using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class BillingInformationMapper
{
    public static IpaffsDataApi.BillingInformation Map(BillingInformation? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.BillingInformation
        {
            IsConfirmed = from.IsConfirmed,
            EmailAddress = from.EmailAddress,
            PhoneNumber = from.PhoneNumber,
            ContactName = from.ContactName,
            PostalAddress = PostalAddressMapper.Map(from.PostalAddress),
        };

        return to;
    }
}
