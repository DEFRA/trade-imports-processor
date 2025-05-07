using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ContactDetailsMapper
{
    public static IpaffsDataApi.ContactDetails Map(ContactDetails? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ContactDetails
        {
            Name = from.Name,
            Telephone = from.Telephone,
            Email = from.Email,
            Agent = from.Agent,
        };

        return to;
    }
}
