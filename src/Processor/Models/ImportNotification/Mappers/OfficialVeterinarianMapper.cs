using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class OfficialVeterinarianMapper
{
    public static IpaffsDataApi.OfficialVeterinarian Map(OfficialVeterinarian? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.OfficialVeterinarian();
        to.FirstName = from?.FirstName;
        to.LastName = from?.LastName;
        to.Email = from?.Email;
        to.Phone = from?.Phone;
        to.Fax = from?.Fax;
        to.Signed = from?.Signed;
        return to;
    }
}
