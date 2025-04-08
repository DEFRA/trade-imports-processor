using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ConsignmentCheckIdentityCheckTypeEnumMapper
{
    public static IpaffsDataApi.ConsignmentCheckIdentityCheckType? Map(ConsignmentCheckIdentityCheckType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ConsignmentCheckIdentityCheckType.SealCheck => IpaffsDataApi.ConsignmentCheckIdentityCheckType.SealCheck,
            ConsignmentCheckIdentityCheckType.FullIdentityCheck => IpaffsDataApi
                .ConsignmentCheckIdentityCheckType
                .FullIdentityCheck,
            ConsignmentCheckIdentityCheckType.NotDone => IpaffsDataApi.ConsignmentCheckIdentityCheckType.NotDone,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
