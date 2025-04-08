#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposeInternalMarketPurposeEnumMapper
{
    public static IpaffsDataApi.PurposeInternalMarketPurpose? Map(PurposeInternalMarketPurpose? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PurposeInternalMarketPurpose.AnimalFeedingStuff => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .AnimalFeedingStuff,
            PurposeInternalMarketPurpose.HumanConsumption => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .HumanConsumption,
            PurposeInternalMarketPurpose.PharmaceuticalUse => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .PharmaceuticalUse,
            PurposeInternalMarketPurpose.TechnicalUse => IpaffsDataApi.PurposeInternalMarketPurpose.TechnicalUse,
            PurposeInternalMarketPurpose.Other => IpaffsDataApi.PurposeInternalMarketPurpose.Other,
            PurposeInternalMarketPurpose.CommercialSale => IpaffsDataApi.PurposeInternalMarketPurpose.CommercialSale,
            PurposeInternalMarketPurpose.CommercialSaleOrChangeOfOwnership => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .CommercialSaleOrChangeOfOwnership,
            PurposeInternalMarketPurpose.Rescue => IpaffsDataApi.PurposeInternalMarketPurpose.Rescue,
            PurposeInternalMarketPurpose.Breeding => IpaffsDataApi.PurposeInternalMarketPurpose.Breeding,
            PurposeInternalMarketPurpose.Research => IpaffsDataApi.PurposeInternalMarketPurpose.Research,
            PurposeInternalMarketPurpose.RacingOrCompetition => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .RacingOrCompetition,
            PurposeInternalMarketPurpose.ApprovedPremisesOrBody => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .ApprovedPremisesOrBody,
            PurposeInternalMarketPurpose.CompanionAnimalNotForResaleOrRehoming => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .CompanionAnimalNotForResaleOrRehoming,
            PurposeInternalMarketPurpose.Production => IpaffsDataApi.PurposeInternalMarketPurpose.Production,
            PurposeInternalMarketPurpose.Slaughter => IpaffsDataApi.PurposeInternalMarketPurpose.Slaughter,
            PurposeInternalMarketPurpose.Fattening => IpaffsDataApi.PurposeInternalMarketPurpose.Fattening,
            PurposeInternalMarketPurpose.GameRestocking => IpaffsDataApi.PurposeInternalMarketPurpose.GameRestocking,
            PurposeInternalMarketPurpose.RegisteredHorses => IpaffsDataApi
                .PurposeInternalMarketPurpose
                .RegisteredHorses,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
