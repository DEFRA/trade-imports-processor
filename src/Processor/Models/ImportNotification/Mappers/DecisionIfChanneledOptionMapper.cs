#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionIfChanneledOptionEnumMapper
{
    public static IpaffsDataApi.DecisionIfChanneledOption? Map(DecisionIfChanneledOption? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionIfChanneledOption.Article8 => IpaffsDataApi.DecisionIfChanneledOption.Article8,
            DecisionIfChanneledOption.Article15 => IpaffsDataApi.DecisionIfChanneledOption.Article15,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
