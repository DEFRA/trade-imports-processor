using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class CommodityComplementMapper
{
    public static IpaffsDataApi.CommodityComplement Map(CommodityComplement? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.CommodityComplement
        {
            UniqueComplementId = from.UniqueComplementId,
            CommodityDescription = from.CommodityDescription,
            CommodityId = from.CommodityId,
            ComplementId = from.ComplementId,
            ComplementName = from.ComplementName,
            EppoCode = from.EppoCode,
            IsWoodPackaging = from.IsWoodPackaging,
            SpeciesId = from.SpeciesId,
            SpeciesName = from.SpeciesName,
            SpeciesNomination = from.SpeciesNomination,
            SpeciesTypeName = from.SpeciesTypeName,
            SpeciesType = from.SpeciesType,
            SpeciesClassName = from.SpeciesClassName,
            SpeciesClass = from.SpeciesClass,
            SpeciesFamilyName = from.SpeciesFamilyName,
            SpeciesFamily = from.SpeciesFamily,
            SpeciesCommonName = from.SpeciesCommonName,
            IsCdsMatched = from.IsCdsMatched,
        };

        return to;
    }
}
