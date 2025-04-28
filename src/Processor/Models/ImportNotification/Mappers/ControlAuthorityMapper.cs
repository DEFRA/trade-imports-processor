using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ControlAuthorityMapper
{
    public static IpaffsDataApi.ControlAuthority Map(ControlAuthority? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ControlAuthority();
        to.OfficialVeterinarian = OfficialVeterinarianMapper.Map(from.OfficialVeterinarian);
        to.CustomsReferenceNo = from.CustomsReferenceNo;
        to.ContainerResealed = from.ContainerResealed;
        to.NewSealNumber = from.NewSealNumber;
        to.IuuFishingReference = from.IuuFishingReference;
        to.IuuCheckRequired = from.IuuCheckRequired;
        to.IuuOption = ControlAuthorityIuuOptionEnumMapper.Map(from.IuuOption);
        return to;
    }
}
