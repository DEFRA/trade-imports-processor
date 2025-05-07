using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class JourneyRiskCategorisationResultMapper
{
    public static IpaffsDataApi.JourneyRiskCategorisationResult Map(JourneyRiskCategorisationResult? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.JourneyRiskCategorisationResult
        {
            RiskLevel = from.RiskLevel,
            RiskLevelMethod = from.RiskLevelMethod,
            RiskLevelSetFor = from.RiskLevelSetFor,
        };

        return to;
    }
}
