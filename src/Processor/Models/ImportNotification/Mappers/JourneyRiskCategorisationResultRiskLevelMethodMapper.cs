#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class JourneyRiskCategorisationResultRiskLevelMethodEnumMapper
{
    public static IpaffsDataApi.JourneyRiskCategorisationResultRiskLevelMethod? Map(
        JourneyRiskCategorisationResultRiskLevelMethod? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            JourneyRiskCategorisationResultRiskLevelMethod.System => IpaffsDataApi
                .JourneyRiskCategorisationResultRiskLevelMethod
                .System,
            JourneyRiskCategorisationResultRiskLevelMethod.User => IpaffsDataApi
                .JourneyRiskCategorisationResultRiskLevelMethod
                .User,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
