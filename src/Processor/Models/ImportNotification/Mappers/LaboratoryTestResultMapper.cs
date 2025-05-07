using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestResultMapper
{
    public static IpaffsDataApi.LaboratoryTestResult Map(LaboratoryTestResult? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.LaboratoryTestResult
        {
            SampleUseByDate = from.SampleUseByDate,
            ReleasedOn = from.ReleasedDate,
            LaboratoryTestMethod = from.LaboratoryTestMethod,
            Results = from.Results,
            Conclusion = from.Conclusion,
            LabTestCreatedOn = from.LabTestCreatedDate,
        };

        return to;
    }
}
