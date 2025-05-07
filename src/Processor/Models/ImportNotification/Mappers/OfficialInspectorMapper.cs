using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class OfficialInspectorMapper
{
    public static IpaffsDataApi.OfficialInspector Map(OfficialInspector? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.OfficialInspector
        {
            FirstName = from.FirstName,
            LastName = from.LastName,
            Email = from.Email,
            Phone = from.Phone,
            Fax = from.Fax,
            Address = AddressMapper.Map(from.Address),
            Signed = from.Signed,
        };

        return to;
    }
}
