using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectorMapper
{
    public static IpaffsDataApi.Inspector Map(Inspector? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Inspector
        {
            Name = from.Name,
            Phone = from.Phone,
            Email = from.Email,
        };

        return to;
    }
}
