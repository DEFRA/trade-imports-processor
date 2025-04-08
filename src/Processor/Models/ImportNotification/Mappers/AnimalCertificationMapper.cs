using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class AnimalCertificationMapper
{
    public static IpaffsDataApi.AnimalCertification? Map(AnimalCertification? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            AnimalCertification.AnimalFeedingStuff => IpaffsDataApi.AnimalCertification.AnimalFeedingStuff,
            AnimalCertification.Approved => IpaffsDataApi.AnimalCertification.Approved,
            AnimalCertification.ArtificialReproduction => IpaffsDataApi.AnimalCertification.ArtificialReproduction,
            AnimalCertification.Breeding => IpaffsDataApi.AnimalCertification.Breeding,
            AnimalCertification.Circus => IpaffsDataApi.AnimalCertification.Circus,
            AnimalCertification.CommercialSale => IpaffsDataApi.AnimalCertification.CommercialSale,
            AnimalCertification.CommercialSaleOrChangeOfOwnership => IpaffsDataApi
                .AnimalCertification
                .CommercialSaleOrChangeOfOwnership,
            AnimalCertification.Fattening => IpaffsDataApi.AnimalCertification.Fattening,
            AnimalCertification.GameRestocking => IpaffsDataApi.AnimalCertification.GameRestocking,
            AnimalCertification.HumanConsumption => IpaffsDataApi.AnimalCertification.HumanConsumption,
            AnimalCertification.InternalMarket => IpaffsDataApi.AnimalCertification.InternalMarket,
            AnimalCertification.Other => IpaffsDataApi.AnimalCertification.Other,
            AnimalCertification.PersonallyOwnedPetsNotForRehoming => IpaffsDataApi
                .AnimalCertification
                .PersonallyOwnedPetsNotForRehoming,
            AnimalCertification.Pets => IpaffsDataApi.AnimalCertification.Pets,
            AnimalCertification.Production => IpaffsDataApi.AnimalCertification.Production,
            AnimalCertification.Quarantine => IpaffsDataApi.AnimalCertification.Quarantine,
            AnimalCertification.RacingCompetition => IpaffsDataApi.AnimalCertification.RacingCompetition,
            AnimalCertification.RegisteredEquidae => IpaffsDataApi.AnimalCertification.RegisteredEquidae,
            AnimalCertification.Registered => IpaffsDataApi.AnimalCertification.Registered,
            AnimalCertification.RejectedOrReturnedConsignment => IpaffsDataApi
                .AnimalCertification
                .RejectedOrReturnedConsignment,
            AnimalCertification.Relaying => IpaffsDataApi.AnimalCertification.Relaying,
            AnimalCertification.RescueRehoming => IpaffsDataApi.AnimalCertification.RescueRehoming,
            AnimalCertification.Research => IpaffsDataApi.AnimalCertification.Research,
            AnimalCertification.Slaughter => IpaffsDataApi.AnimalCertification.Slaughter,
            AnimalCertification.TechnicalPharmaceuticalUse => IpaffsDataApi
                .AnimalCertification
                .TechnicalPharmaceuticalUse,
            AnimalCertification.Transit => IpaffsDataApi.AnimalCertification.Transit,
            AnimalCertification.ZooCollection => IpaffsDataApi.AnimalCertification.ZooCollection,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
