using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class LaboratoryTestsMapper
{
    public static IpaffsDataApi.LaboratoryTests Map(LaboratoryTests? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.LaboratoryTests();
        to.TestedOn = from.TestDate;
        to.TestReason = LaboratoryTestsTestReasonEnumMapper.Map(from.TestReason);
        to.SingleLaboratoryTests = from
            ?.SingleLaboratoryTests?.Select(x => SingleLaboratoryTestMapper.Map(x))
            .ToArray();
        return to;
    }
}
