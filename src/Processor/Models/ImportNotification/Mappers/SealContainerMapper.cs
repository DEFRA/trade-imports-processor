using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SealContainerMapper
{
    public static IpaffsDataApi.SealContainer Map(SealContainer? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.SealContainer();
        to.SealNumber = from.SealNumber;
        to.ContainerNumber = from.ContainerNumber;
        to.OfficialSeal = from.OfficialSeal;
        to.ResealedSealNumber = from.ResealedSealNumber;
        return to;
    }
}
