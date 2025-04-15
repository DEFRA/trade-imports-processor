using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImpactOfTransportOnAnimalsMapper
{
    public static IpaffsDataApi.ImpactOfTransportOnAnimals Map(ImpactOfTransportOnAnimals? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ImpactOfTransportOnAnimals();
        to.NumberOfDeadAnimals = from?.NumberOfDeadAnimals;
        to.NumberOfDeadAnimalsUnit = from?.NumberOfDeadAnimalsUnit;
        to.NumberOfUnfitAnimals = from?.NumberOfUnfitAnimals;
        to.NumberOfUnfitAnimalsUnit = from?.NumberOfUnfitAnimalsUnit;
        to.NumberOfBirthOrAbortion = from?.NumberOfBirthOrAbortion;
        return to;
    }
}
