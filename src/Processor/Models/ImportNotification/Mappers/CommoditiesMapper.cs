using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommoditiesMapper
{
    public static IpaffsDataApi.Commodities Map(Commodities? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Commodities();
        to.GmsDeclarationAccepted = from?.GmsDeclarationAccepted;
        to.ConsignedCountryInChargeGroup = from?.ConsignedCountryInChargeGroup;
        to.TotalGrossWeight = from?.TotalGrossWeight;
        to.TotalNetWeight = from?.TotalNetWeight;
        to.TotalGrossVolume = from?.TotalGrossVolume;
        to.TotalGrossVolumeUnit = from?.TotalGrossVolumeUnit;
        to.NumberOfPackages = from?.NumberOfPackages;
        to.Temperature = from?.Temperature;
        to.NumberOfAnimals = from?.NumberOfAnimals;
        to.IncludeNonAblactedAnimals = from?.IncludeNonAblactedAnimals;
        to.CountryOfOrigin = from?.CountryOfOrigin;
        to.CountryOfOriginIsPodCountry = from?.CountryOfOriginIsPodCountry;
        to.IsLowRiskArticle72Country = from?.IsLowRiskArticle72Country;
        to.RegionOfOrigin = from?.RegionOfOrigin;
        to.ConsignedCountry = from?.ConsignedCountry;
        to.AnimalsCertifiedAs = from?.AnimalsCertifiedAs;
        to.CommodityIntendedFor = CommoditiesCommodityIntendedForMapper.Map(from?.CommodityIntendedFor);
        return to;
    }
}
