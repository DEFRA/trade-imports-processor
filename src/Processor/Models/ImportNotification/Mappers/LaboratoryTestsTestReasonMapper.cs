using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestsTestReasonEnumMapper
{
    public static IpaffsDataApi.LaboratoryTestsTestReason? Map(LaboratoryTestsTestReason? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            LaboratoryTestsTestReason.Random => IpaffsDataApi.LaboratoryTestsTestReason.Random,
            LaboratoryTestsTestReason.Suspicious => IpaffsDataApi.LaboratoryTestsTestReason.Suspicious,
            LaboratoryTestsTestReason.ReEnforced => IpaffsDataApi.LaboratoryTestsTestReason.ReEnforced,
            LaboratoryTestsTestReason.IntensifiedControls => IpaffsDataApi
                .LaboratoryTestsTestReason
                .IntensifiedControls,
            LaboratoryTestsTestReason.Required => IpaffsDataApi.LaboratoryTestsTestReason.Required,
            LaboratoryTestsTestReason.LatentInfectionSampling => IpaffsDataApi
                .LaboratoryTestsTestReason
                .LatentInfectionSampling,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
