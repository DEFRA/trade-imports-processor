#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class RiskAssessmentResultMapper
{
    public static IpaffsDataApi.RiskAssessmentResult Map(RiskAssessmentResult? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.RiskAssessmentResult();
        to.CommodityResults = from?.CommodityResults?.Select(x => CommodityRiskResultMapper.Map(x)).ToArray();
        to.AssessedOn = from?.AssessmentDateTime;
        return to;
    }
}
