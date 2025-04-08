#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PurposeForNonConformingEnumMapper
{
    public static IpaffsDataApi.PurposeForNonConforming? Map(PurposeForNonConforming? from)
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            PurposeForNonConforming.CustomsWarehouse => IpaffsDataApi.PurposeForNonConforming.CustomsWarehouse,
            PurposeForNonConforming.FreeZoneOrFreeWarehouse => IpaffsDataApi
                .PurposeForNonConforming
                .FreeZoneOrFreeWarehouse,
            PurposeForNonConforming.ShipSupplier => IpaffsDataApi.PurposeForNonConforming.ShipSupplier,
            PurposeForNonConforming.Ship => IpaffsDataApi.PurposeForNonConforming.Ship,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
