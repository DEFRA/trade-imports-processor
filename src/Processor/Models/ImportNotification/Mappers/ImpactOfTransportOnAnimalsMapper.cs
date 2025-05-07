using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImpactOfTransportOnAnimalsMapper
{
    public static IpaffsDataApi.ImpactOfTransportOnAnimals Map(ImpactOfTransportOnAnimals? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ImpactOfTransportOnAnimals
        {
            NumberOfDeadAnimals = from.NumberOfDeadAnimals,
            NumberOfDeadAnimalsUnit = from.NumberOfDeadAnimalsUnit,
            NumberOfUnfitAnimals = from.NumberOfUnfitAnimals,
            NumberOfUnfitAnimalsUnit = from.NumberOfUnfitAnimalsUnit,
            NumberOfBirthOrAbortion = from.NumberOfBirthOrAbortion,
        };

        return to;
    }
}
