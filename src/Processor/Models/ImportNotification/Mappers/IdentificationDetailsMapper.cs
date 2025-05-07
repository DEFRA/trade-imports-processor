using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class IdentificationDetailsMapper
{
    public static IpaffsDataApi.IdentificationDetails Map(IdentificationDetails? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.IdentificationDetails
        {
            IdentificationDetail = from.IdentificationDetail,
            IdentificationDescription = from.IdentificationDescription,
        };

        return to;
    }
}
