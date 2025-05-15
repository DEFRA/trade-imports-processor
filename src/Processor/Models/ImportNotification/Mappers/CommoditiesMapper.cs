using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommoditiesMapper
{
    public static IpaffsDataApi.Commodities Map(Commodities? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Commodities
        {
            GmsDeclarationAccepted = from.GmsDeclarationAccepted,
            ConsignedCountryInChargeGroup = from.ConsignedCountryInChargeGroup,
            TotalGrossWeight = from.TotalGrossWeight,
            TotalNetWeight = from.TotalNetWeight,
            TotalGrossVolume = from.TotalGrossVolume,
            TotalGrossVolumeUnit = from.TotalGrossVolumeUnit,
            NumberOfPackages = from.NumberOfPackages,
            Temperature = from.Temperature,
            NumberOfAnimals = from.NumberOfAnimals,
            IncludeNonAblactedAnimals = from.IncludeNonAblactedAnimals,
            CountryOfOrigin = from.CountryOfOrigin,
            CountryOfOriginIsPodCountry = from.CountryOfOriginIsPodCountry,
            IsLowRiskArticle72Country = from.IsLowRiskArticle72Country,
            RegionOfOrigin = from.RegionOfOrigin,
            ConsignedCountry = from.ConsignedCountry,
            AnimalsCertifiedAs = from.AnimalsCertifiedAs,
            CommodityIntendedFor = from.CommodityIntendedFor,
            CommodityComplements = from.CommodityComplements?.Select(CommodityComplementMapper.Map).ToArray(),
            ComplementParameterSets = from.ComplementParameterSets?.Select(ComplementParameterSetMapper.Map).ToArray(),
        };

        return to;
    }
}
