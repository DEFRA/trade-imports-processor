using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class KeyDataPairMapper
{
    public static IpaffsDataApi.KeyDataPair Map(KeyDataPair? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.KeyDataPair();
        to.Key = from.Key;
        to.Data = from.Data;
        return to;
    }
}
