using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApplicantMapper
{
    public static IpaffsDataApi.Applicant Map(Applicant? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Applicant();
        to.Laboratory = from?.Laboratory;
        to.LaboratoryAddress = from?.LaboratoryAddress;
        to.LaboratoryIdentification = from?.LaboratoryIdentification;
        to.LaboratoryPhoneNumber = from?.LaboratoryPhoneNumber;
        to.LaboratoryEmail = from?.LaboratoryEmail;
        to.SampleBatchNumber = from?.SampleBatchNumber;
        to.AnalysisType = ApplicantAnalysisTypeMapper.Map(from?.AnalysisType);
        to.NumberOfSamples = from?.NumberOfSamples;
        to.SampleType = from?.SampleType;
        to.ConservationOfSample = ApplicantConservationOfSampleMapper.Map(from?.ConservationOfSample);
        to.Inspector = InspectorMapper.Map(from?.Inspector);
        to.SampledOn = DateTimeMapper.Map(from?.SampleDate, from?.SampleTime);
        return to;
    }
}
