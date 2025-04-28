using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommonUserChargeMapper
{
    public static IpaffsDataApi.CommonUserCharge Map(CommonUserCharge? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.CommonUserCharge();
        to.WasSentToTradeCharge = from.WasSentToTradeCharge;
        return to;
    }
}
