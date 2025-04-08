#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class NominatedContactMapper
{
    public static IpaffsDataApi.NominatedContact Map(NominatedContact? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.NominatedContact();
        to.Name = from?.Name;
        to.Email = from?.Email;
        to.Telephone = from?.Telephone;
        return to;
    }
}
