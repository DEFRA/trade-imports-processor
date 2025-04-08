#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class TemperatureEnumMapper
{
    public static IpaffsDataApi.Temperature? Map(Temperature? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            Temperature.Ambient => IpaffsDataApi.Temperature.Ambient,
            Temperature.Chilled => IpaffsDataApi.Temperature.Chilled,
            Temperature.Frozen => IpaffsDataApi.Temperature.Frozen,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
