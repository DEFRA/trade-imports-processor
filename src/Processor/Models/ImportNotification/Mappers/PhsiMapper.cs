using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PhsiMapper
{
    public static IpaffsDataApi.Phsi Map(Phsi? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Phsi();
        to.DocumentCheck = from?.DocumentCheck;
        to.IdentityCheck = from?.IdentityCheck;
        to.PhysicalCheck = from?.PhysicalCheck;
        return to;
    }
}
