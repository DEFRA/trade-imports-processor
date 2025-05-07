using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestsMapper
{
    public static IpaffsDataApi.LaboratoryTests Map(LaboratoryTests? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.LaboratoryTests
        {
            TestedOn = from.TestDate,
            TestReason = from.TestReason,
            SingleLaboratoryTests = from
                ?.SingleLaboratoryTests?.Select(x => SingleLaboratoryTestMapper.Map(x))
                .ToArray(),
        };

        return to;
    }
}
