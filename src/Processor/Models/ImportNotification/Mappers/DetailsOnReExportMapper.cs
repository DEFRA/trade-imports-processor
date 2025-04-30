using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DetailsOnReExportMapper
{
    public static IpaffsDataApi.DetailsOnReExport Map(DetailsOnReExport? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.DetailsOnReExport();
        to.Date = from.Date;
        to.MeansOfTransportNo = from.MeansOfTransportNo;
        to.TransportType = from.TransportType;
        to.Document = from.Document;
        to.CountryOfReDispatching = from.CountryOfReDispatching;
        to.ExitBip = from.ExitBip;
        return to;
    }
}
