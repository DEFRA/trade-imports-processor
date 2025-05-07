using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionCheckMapper
{
    public static IpaffsDataApi.InspectionCheck Map(InspectionCheck? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.InspectionCheck
        {
            Type = from.Type,
            Status = from.Status,
            Reason = from.Reason,
            OtherReason = from.OtherReason,
            IsSelectedForChecks = from.IsSelectedForChecks,
            HasChecksComplete = from.HasChecksComplete,
        };

        return to;
    }
}
