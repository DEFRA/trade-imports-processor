using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ContactDetailsMapper
{
    public static IpaffsDataApi.ContactDetails Map(ContactDetails? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ContactDetails();
        to.Name = from.Name;
        to.Telephone = from.Telephone;
        to.Email = from.Email;
        to.Agent = from.Agent;
        return to;
    }
}
