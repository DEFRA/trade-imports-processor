using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class IdentifiersMapper
{
    public static IpaffsDataApi.Identifiers Map(Identifiers? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Identifiers
        {
            SpeciesNumber = from.SpeciesNumber,
            Data = from.Data,
            IsPlaceOfDestinationThePermanentAddress = from.IsPlaceOfDestinationThePermanentAddress,
            PermanentAddress = EconomicOperatorMapper.Map(from.PermanentAddress),
        };

        return to;
    }
}
