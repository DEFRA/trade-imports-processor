#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlAuthorityIuuOptionEnumMapper
{
    public static IpaffsDataApi.ControlAuthorityIuuOption? Map(ControlAuthorityIuuOption? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ControlAuthorityIuuOption.Iuuok => IpaffsDataApi.ControlAuthorityIuuOption.Iuuok,
            ControlAuthorityIuuOption.Iuuna => IpaffsDataApi.ControlAuthorityIuuOption.Iuuna,
            ControlAuthorityIuuOption.IUUNotCompliant => IpaffsDataApi.ControlAuthorityIuuOption.IUUNotCompliant,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
