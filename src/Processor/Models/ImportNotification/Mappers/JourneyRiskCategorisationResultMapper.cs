using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class JourneyRiskCategorisationResultMapper
{
    public static IpaffsDataApi.JourneyRiskCategorisationResult Map(JourneyRiskCategorisationResult? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.JourneyRiskCategorisationResult();
        to.RiskLevel = from.RiskLevel;
        to.RiskLevelMethod = from.RiskLevelMethod;
        to.RiskLevelSetFor = from.RiskLevelSetFor;
        return to;
    }
}
