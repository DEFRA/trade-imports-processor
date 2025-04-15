using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SealCheckMapper
{
    public static IpaffsDataApi.SealCheck Map(SealCheck? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.SealCheck();
        to.Satisfactory = from?.Satisfactory;
        to.Reason = from?.Reason;
        to.OfficialInspector = OfficialInspectorMapper.Map(from?.OfficialInspector);
        to.CheckedOn = from?.DateTimeOfCheck;
        return to;
    }
}
