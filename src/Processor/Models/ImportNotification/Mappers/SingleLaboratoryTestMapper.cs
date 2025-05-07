using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SingleLaboratoryTestMapper
{
    public static IpaffsDataApi.SingleLaboratoryTest Map(SingleLaboratoryTest? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.SingleLaboratoryTest
        {
            CommodityCode = from.CommodityCode,
            SpeciesId = from.SpeciesId,
            TracesId = from.TracesId,
            TestName = from.TestName,
            Applicant = ApplicantMapper.Map(from.Applicant),
            LaboratoryTestResult = LaboratoryTestResultMapper.Map(from.LaboratoryTestResult),
        };

        return to;
    }
}
