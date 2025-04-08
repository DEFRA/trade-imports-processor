#nullable enable


using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionSpecificWarehouseNonConformingConsignmentEnumMapper
{
    public static IpaffsDataApi.DecisionSpecificWarehouseNonConformingConsignment? Map(
        DecisionSpecificWarehouseNonConformingConsignment? from
    )
    {
        if (from == null)
        {
            return default!;
        }
        return from switch
        {
            DecisionSpecificWarehouseNonConformingConsignment.CustomWarehouse => IpaffsDataApi
                .DecisionSpecificWarehouseNonConformingConsignment
                .CustomWarehouse,
            DecisionSpecificWarehouseNonConformingConsignment.FreeZoneOrFreeWarehouse => IpaffsDataApi
                .DecisionSpecificWarehouseNonConformingConsignment
                .FreeZoneOrFreeWarehouse,
            DecisionSpecificWarehouseNonConformingConsignment.ShipSupplier => IpaffsDataApi
                .DecisionSpecificWarehouseNonConformingConsignment
                .ShipSupplier,
            DecisionSpecificWarehouseNonConformingConsignment.Ship => IpaffsDataApi
                .DecisionSpecificWarehouseNonConformingConsignment
                .Ship,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
