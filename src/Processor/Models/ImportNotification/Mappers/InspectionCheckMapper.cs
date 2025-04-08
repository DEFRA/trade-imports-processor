#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionCheckMapper
{
    public static IpaffsDataApi.InspectionCheck Map(InspectionCheck? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.InspectionCheck();
        to.Type = InspectionCheckTypeEnumMapper.Map(from?.Type);
        to.Status = InspectionCheckStatusEnumMapper.Map(from?.Status);
        to.Reason = from?.Reason;
        to.OtherReason = from?.OtherReason;
        to.IsSelectedForChecks = from?.IsSelectedForChecks;
        to.HasChecksComplete = from?.HasChecksComplete;
        return to;
    }
}
