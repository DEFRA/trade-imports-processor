using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionFreeCirculationPurposeEnumMapper
{
    public static IpaffsDataApi.DecisionFreeCirculationPurpose? Map(DecisionFreeCirculationPurpose? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            DecisionFreeCirculationPurpose.AnimalFeedingStuff => IpaffsDataApi
                .DecisionFreeCirculationPurpose
                .AnimalFeedingStuff,
            DecisionFreeCirculationPurpose.HumanConsumption => IpaffsDataApi
                .DecisionFreeCirculationPurpose
                .HumanConsumption,
            DecisionFreeCirculationPurpose.PharmaceuticalUse => IpaffsDataApi
                .DecisionFreeCirculationPurpose
                .PharmaceuticalUse,
            DecisionFreeCirculationPurpose.TechnicalUse => IpaffsDataApi.DecisionFreeCirculationPurpose.TechnicalUse,
            DecisionFreeCirculationPurpose.FurtherProcess => IpaffsDataApi
                .DecisionFreeCirculationPurpose
                .FurtherProcess,
            DecisionFreeCirculationPurpose.Other => IpaffsDataApi.DecisionFreeCirculationPurpose.Other,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
