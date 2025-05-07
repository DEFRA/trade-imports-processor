using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DetailsOnReExportMapper
{
    public static IpaffsDataApi.DetailsOnReExport Map(DetailsOnReExport? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.DetailsOnReExport
        {
            Date = from.Date,
            MeansOfTransportNo = from.MeansOfTransportNo,
            TransportType = from.TransportType,
            Document = from.Document,
            CountryOfReDispatching = from.CountryOfReDispatching,
            ExitBip = from.ExitBip,
        };

        return to;
    }
}
