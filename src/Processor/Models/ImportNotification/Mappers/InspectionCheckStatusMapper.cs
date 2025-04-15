using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionCheckStatusEnumMapper
{
    public static IpaffsDataApi.InspectionCheckStatus? Map(InspectionCheckStatus? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            InspectionCheckStatus.ToDo => IpaffsDataApi.InspectionCheckStatus.ToDo,
            InspectionCheckStatus.Compliant => IpaffsDataApi.InspectionCheckStatus.Compliant,
            InspectionCheckStatus.AutoCleared => IpaffsDataApi.InspectionCheckStatus.AutoCleared,
            InspectionCheckStatus.NonCompliant => IpaffsDataApi.InspectionCheckStatus.NonCompliant,
            InspectionCheckStatus.NotInspected => IpaffsDataApi.InspectionCheckStatus.NotInspected,
            InspectionCheckStatus.ToBeInspected => IpaffsDataApi.InspectionCheckStatus.ToBeInspected,
            InspectionCheckStatus.Hold => IpaffsDataApi.InspectionCheckStatus.Hold,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
