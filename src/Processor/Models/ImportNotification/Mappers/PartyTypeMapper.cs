#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartyTypeEnumMapper
{
    public static IpaffsDataApi.PartyType? Map(PartyType? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PartyType.CommercialTransporter => IpaffsDataApi.PartyType.CommercialTransporter,
            PartyType.PrivateTransporter => IpaffsDataApi.PartyType.PrivateTransporter,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
