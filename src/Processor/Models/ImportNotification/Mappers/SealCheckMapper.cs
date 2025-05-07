using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SealCheckMapper
{
    public static IpaffsDataApi.SealCheck Map(SealCheck? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.SealCheck
        {
            Satisfactory = from.Satisfactory,
            Reason = from.Reason,
            OfficialInspector = OfficialInspectorMapper.Map(from.OfficialInspector),
            CheckedOn = from.DateTimeOfCheck,
        };

        return to;
    }
}
