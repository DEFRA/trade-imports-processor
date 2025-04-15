using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class JourneyRiskCategorisationResultRiskLevelEnumMapper
{
    public static IpaffsDataApi.JourneyRiskCategorisationResultRiskLevel? Map(
        JourneyRiskCategorisationResultRiskLevel? from
    )
    {
        if (from == null)
            return default!;
        return from switch
        {
            JourneyRiskCategorisationResultRiskLevel.High => IpaffsDataApi
                .JourneyRiskCategorisationResultRiskLevel
                .High,
            JourneyRiskCategorisationResultRiskLevel.Medium => IpaffsDataApi
                .JourneyRiskCategorisationResultRiskLevel
                .Medium,
            JourneyRiskCategorisationResultRiskLevel.Low => IpaffsDataApi.JourneyRiskCategorisationResultRiskLevel.Low,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
