using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SingleLaboratoryTestMapper
{
    public static IpaffsDataApi.SingleLaboratoryTest Map(SingleLaboratoryTest? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.SingleLaboratoryTest();
        to.CommodityCode = from?.CommodityCode;
        to.SpeciesId = from?.SpeciesId;
        to.TracesId = from?.TracesId;
        to.TestName = from?.TestName;
        to.Applicant = ApplicantMapper.Map(from?.Applicant);
        to.LaboratoryTestResult = LaboratoryTestResultMapper.Map(from?.LaboratoryTestResult);
        return to;
    }
}
