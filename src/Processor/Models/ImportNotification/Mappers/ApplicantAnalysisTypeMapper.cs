#nullable enable

using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApplicantAnalysisTypeMapper
{
    public static IpaffsDataApi.ApplicantAnalysisType? Map(ApplicantAnalysisType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ApplicantAnalysisType.InitialAnalysis => IpaffsDataApi.ApplicantAnalysisType.InitialAnalysis,
            ApplicantAnalysisType.CounterAnalysis => IpaffsDataApi.ApplicantAnalysisType.CounterAnalysis,
            ApplicantAnalysisType.SecondExpertAnalysis => IpaffsDataApi.ApplicantAnalysisType.SecondExpertAnalysis,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
