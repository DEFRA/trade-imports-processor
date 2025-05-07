using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class MeansOfTransportMapper
{
    public static IpaffsDataApi.MeansOfTransport Map(MeansOfTransport? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.MeansOfTransport
        {
            Type = from.Type,
            Document = from.Document,
            Id = from.Id,
        };

        return to;
    }
}
