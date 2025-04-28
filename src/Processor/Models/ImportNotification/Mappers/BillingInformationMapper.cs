using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class BillingInformationMapper
{
    public static IpaffsDataApi.BillingInformation Map(BillingInformation? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.BillingInformation();
        to.IsConfirmed = from.IsConfirmed;
        to.EmailAddress = from.EmailAddress;
        to.PhoneNumber = from.PhoneNumber;
        to.ContactName = from.ContactName;
        to.PostalAddress = PostalAddressMapper.Map(from.PostalAddress);
        return to;
    }
}
