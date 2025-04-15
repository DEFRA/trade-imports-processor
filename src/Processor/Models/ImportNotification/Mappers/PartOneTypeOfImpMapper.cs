using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartOneTypeOfImpEnumMapper
{
    public static IpaffsDataApi.PartOneTypeOfImp? Map(PartOneTypeOfImp? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            PartOneTypeOfImp.A => IpaffsDataApi.PartOneTypeOfImp.A,
            PartOneTypeOfImp.P => IpaffsDataApi.PartOneTypeOfImp.P,
            PartOneTypeOfImp.D => IpaffsDataApi.PartOneTypeOfImp.D,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
