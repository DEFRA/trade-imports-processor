using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ApplicantConservationOfSampleMapper
{
    public static IpaffsDataApi.ApplicantConservationOfSample? Map(ApplicantConservationOfSample? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            ApplicantConservationOfSample.Ambient => IpaffsDataApi.ApplicantConservationOfSample.Ambient,
            ApplicantConservationOfSample.Chilled => IpaffsDataApi.ApplicantConservationOfSample.Chilled,
            ApplicantConservationOfSample.Frozen => IpaffsDataApi.ApplicantConservationOfSample.Frozen,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
