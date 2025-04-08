#nullable enable




using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class InspectionOverrideMapper
{
    public static IpaffsDataApi.InspectionOverride Map(InspectionOverride? from)
    {
        if (from is null)
        {
            return default!;
        }
        var to = new IpaffsDataApi.InspectionOverride();
        to.OriginalDecision = from?.OriginalDecision;
        to.OverriddenOn = from?.OverriddenOn;
        to.OverriddenBy = UserInformationMapper.Map(from?.OverriddenBy);
        return to;
    }
}
