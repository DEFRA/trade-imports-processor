using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartyMapper
{
    public static IpaffsDataApi.Party Map(Party? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Party
        {
            Id = from.Id,
            Name = from.Name,
            CompanyId = from.CompanyId,
            ContactId = from.ContactId,
            CompanyName = from.CompanyName,
            Addresses = from.Addresses,
            County = from.County,
            PostCode = from.PostCode,
            Country = from.Country,
            City = from.City,
            TracesId = from.TracesId,
            Type = from.Type,
            ApprovalNumber = from.ApprovalNumber,
            Phone = from.Phone,
            Fax = from.Fax,
            Email = from.Email,
        };

        return to;
    }
}
