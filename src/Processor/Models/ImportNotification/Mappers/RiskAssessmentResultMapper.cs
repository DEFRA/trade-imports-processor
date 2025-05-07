using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class RiskAssessmentResultMapper
{
    public static IpaffsDataApi.RiskAssessmentResult Map(RiskAssessmentResult? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.RiskAssessmentResult
        {
            CommodityResults = from.CommodityResults?.Select(x => CommodityRiskResultMapper.Map(x)).ToArray(),
            AssessedOn = from.AssessmentDateTime,
        };

        return to;
    }
}
