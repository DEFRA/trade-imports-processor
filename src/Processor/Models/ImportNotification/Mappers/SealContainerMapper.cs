using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class SealContainerMapper
{
    public static IpaffsDataApi.SealContainer Map(SealContainer? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.SealContainer
        {
            SealNumber = from.SealNumber,
            ContainerNumber = from.ContainerNumber,
            OfficialSeal = from.OfficialSeal,
            ResealedSealNumber = from.ResealedSealNumber,
        };

        return to;
    }
}
