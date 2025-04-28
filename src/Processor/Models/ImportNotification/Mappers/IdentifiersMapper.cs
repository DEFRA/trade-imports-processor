using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class IdentifiersMapper
{
    public static IpaffsDataApi.Identifiers Map(Identifiers? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Identifiers();
        to.SpeciesNumber = from.SpeciesNumber;
        to.Data = from.Data;
        to.IsPlaceOfDestinationThePermanentAddress = from.IsPlaceOfDestinationThePermanentAddress;
        to.PermanentAddress = EconomicOperatorMapper.Map(from.PermanentAddress);
        return to;
    }
}
