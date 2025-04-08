#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartyMapper
{
    public static IpaffsDataApi.Party Map(Party? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.Party();
        to.Id = from?.Id;
        to.Name = from?.Name;
        to.CompanyId = from?.CompanyId;
        to.ContactId = from?.ContactId;
        to.CompanyName = from?.CompanyName;
        to.Addresses = from?.Addresses;
        to.County = from?.County;
        to.PostCode = from?.PostCode;
        to.Country = from?.Country;
        to.City = from?.City;
        to.TracesId = from?.TracesId;
        to.Type = PartyTypeEnumMapper.Map(from?.Type);
        to.ApprovalNumber = from?.ApprovalNumber;
        to.Phone = from?.Phone;
        to.Fax = from?.Fax;
        to.Email = from?.Email;
        return to;
    }
}
