#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectorMapper
{
    public static IpaffsDataApi.Inspector Map(Inspector? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.Inspector();
        to.Name = from?.Name;
        to.Phone = from?.Phone;
        to.Email = from?.Email;
        return to;
    }
}
