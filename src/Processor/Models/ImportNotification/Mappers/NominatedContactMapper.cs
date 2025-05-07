using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class NominatedContactMapper
{
    public static IpaffsDataApi.NominatedContact Map(NominatedContact? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.NominatedContact
        {
            Name = from.Name,
            Email = from.Email,
            Telephone = from.Telephone,
        };

        return to;
    }
}
