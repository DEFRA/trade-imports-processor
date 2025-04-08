#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class EconomicOperatorStatusEnumMapper
{
    public static IpaffsDataApi.EconomicOperatorStatus? Map(EconomicOperatorStatus? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            EconomicOperatorStatus.Approved => IpaffsDataApi.EconomicOperatorStatus.Approved,
            EconomicOperatorStatus.Nonapproved => IpaffsDataApi.EconomicOperatorStatus.Nonapproved,
            EconomicOperatorStatus.Suspended => IpaffsDataApi.EconomicOperatorStatus.Suspended,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
