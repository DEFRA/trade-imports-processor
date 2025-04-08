using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ConsignmentCheckPhysicalCheckNotDoneReasonEnumMapper
{
    public static IpaffsDataApi.ConsignmentCheckPhysicalCheckNotDoneReason? Map(
        ConsignmentCheckPhysicalCheckNotDoneReason? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ConsignmentCheckPhysicalCheckNotDoneReason.ReducedChecksRegime => IpaffsDataApi
                .ConsignmentCheckPhysicalCheckNotDoneReason
                .ReducedChecksRegime,
            ConsignmentCheckPhysicalCheckNotDoneReason.Other => IpaffsDataApi
                .ConsignmentCheckPhysicalCheckNotDoneReason
                .Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
