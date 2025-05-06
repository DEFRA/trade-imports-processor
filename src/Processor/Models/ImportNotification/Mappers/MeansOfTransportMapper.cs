using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class MeansOfTransportMapper
{
    public static IpaffsDataApi.MeansOfTransport Map(MeansOfTransport? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.MeansOfTransport();
        to.Type = from.Type;
        to.Document = from.Document;
        to.Id = from.Id;
        return to;
    }
}
