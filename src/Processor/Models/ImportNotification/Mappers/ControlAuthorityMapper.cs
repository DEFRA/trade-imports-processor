using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlAuthorityMapper
{
    public static IpaffsDataApi.ControlAuthority Map(ControlAuthority? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ControlAuthority
        {
            OfficialVeterinarian = OfficialVeterinarianMapper.Map(from.OfficialVeterinarian),
            CustomsReferenceNo = from.CustomsReferenceNo,
            ContainerResealed = from.ContainerResealed,
            NewSealNumber = from.NewSealNumber,
            IuuFishingReference = from.IuuFishingReference,
            IuuCheckRequired = from.IuuCheckRequired,
            IuuOption = from.IuuOption,
        };

        return to;
    }
}
