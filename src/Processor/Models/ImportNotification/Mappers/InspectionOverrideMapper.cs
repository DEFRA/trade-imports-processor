using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionOverrideMapper
{
    public static IpaffsDataApi.InspectionOverride Map(InspectionOverride? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.InspectionOverride
        {
            OriginalDecision = from.OriginalDecision,
            OverriddenOn = from.OverriddenOn,
            OverriddenBy = UserInformationMapper.Map(from.OverriddenBy),
        };

        return to;
    }
}
