#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestResultMapper
{
    public static IpaffsDataApi.LaboratoryTestResult Map(LaboratoryTestResult? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.LaboratoryTestResult();
        to.SampleUseByDate = from?.SampleUseByDate;
        to.ReleasedOn = from?.ReleasedDate;
        to.LaboratoryTestMethod = from?.LaboratoryTestMethod;
        to.Results = from?.Results;
        to.Conclusion = LaboratoryTestResultConclusionEnumMapper.Map(from?.Conclusion);
        to.LabTestCreatedOn = from?.LabTestCreatedDate;
        return to;
    }
}
