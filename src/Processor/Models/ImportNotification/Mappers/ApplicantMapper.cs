using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApplicantMapper
{
    public static IpaffsDataApi.Applicant Map(Applicant? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Applicant
        {
            Laboratory = from.Laboratory,
            LaboratoryAddress = from.LaboratoryAddress,
            LaboratoryIdentification = from.LaboratoryIdentification,
            LaboratoryPhoneNumber = from.LaboratoryPhoneNumber,
            LaboratoryEmail = from.LaboratoryEmail,
            SampleBatchNumber = from.SampleBatchNumber,
            AnalysisType = from.AnalysisType,
            NumberOfSamples = from.NumberOfSamples,
            SampleType = from.SampleType,
            ConservationOfSample = from.ConservationOfSample,
            Inspector = InspectorMapper.Map(from.Inspector),
            SampledOn = DateTimeMapper.Map(from.SampleDate, from.SampleTime),
        };

        return to;
    }
}
