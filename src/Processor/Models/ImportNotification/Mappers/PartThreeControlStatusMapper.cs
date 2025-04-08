#nullable enable



using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartThreeControlStatusEnumMapper
{
    public static IpaffsDataApi.PartThreeControlStatus? Map(PartThreeControlStatus? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PartThreeControlStatus.Required => IpaffsDataApi.PartThreeControlStatus.Required,
            PartThreeControlStatus.Completed => IpaffsDataApi.PartThreeControlStatus.Completed,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
