#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class UnitEnumMapper
{
    public static IpaffsDataApi.Unit? Map(Unit? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            Unit.Percent => IpaffsDataApi.Unit.Percent,
            Unit.Number => IpaffsDataApi.Unit.Number,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
