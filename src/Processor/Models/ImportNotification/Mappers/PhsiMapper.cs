using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PhsiMapper
{
    public static IpaffsDataApi.Phsi Map(Phsi? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Phsi
        {
            DocumentCheck = from.DocumentCheck,
            IdentityCheck = from.IdentityCheck,
            PhysicalCheck = from.PhysicalCheck,
        };

        return to;
    }
}
