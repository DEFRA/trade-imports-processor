using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ConsignmentCheckIdentityCheckNotDoneReasonEnumMapper
{
    public static IpaffsDataApi.ConsignmentCheckIdentityCheckNotDoneReason? Map(
        ConsignmentCheckIdentityCheckNotDoneReason? from
    )
    {
        if (from == null)
            return default!;
        return from switch
        {
            ConsignmentCheckIdentityCheckNotDoneReason.ReducedChecksRegime => IpaffsDataApi
                .ConsignmentCheckIdentityCheckNotDoneReason
                .ReducedChecksRegime,
            ConsignmentCheckIdentityCheckNotDoneReason.NotRequired => IpaffsDataApi
                .ConsignmentCheckIdentityCheckNotDoneReason
                .NotRequired,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
