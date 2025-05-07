using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class OfficialVeterinarianMapper
{
    public static IpaffsDataApi.OfficialVeterinarian Map(OfficialVeterinarian? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.OfficialVeterinarian
        {
            FirstName = from.FirstName,
            LastName = from.LastName,
            Email = from.Email,
            Phone = from.Phone,
            Fax = from.Fax,
            Signed = from.Signed,
        };

        return to;
    }
}
