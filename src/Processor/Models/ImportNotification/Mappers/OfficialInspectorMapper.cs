#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class OfficialInspectorMapper
{
    public static IpaffsDataApi.OfficialInspector Map(OfficialInspector? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.OfficialInspector();
        to.FirstName = from?.FirstName;
        to.LastName = from?.LastName;
        to.Email = from?.Email;
        to.Phone = from?.Phone;
        to.Fax = from?.Fax;
        to.Address = AddressMapper.Map(from?.Address);
        to.Signed = from?.Signed;
        return to;
    }
}
