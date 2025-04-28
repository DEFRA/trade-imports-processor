using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class IdentificationDetailsMapper
{
    public static IpaffsDataApi.IdentificationDetails Map(IdentificationDetails? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.IdentificationDetails();
        to.IdentificationDetail = from.IdentificationDetail;
        to.IdentificationDescription = from.IdentificationDescription;
        return to;
    }
}
