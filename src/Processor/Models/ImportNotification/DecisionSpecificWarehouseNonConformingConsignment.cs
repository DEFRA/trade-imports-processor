using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionSpecificWarehouseNonConformingConsignment
{
    [EnumMember(Value = "CustomWarehouse")]
    CustomWarehouse,

    [EnumMember(Value = "FreeZoneOrFreeWarehouse")]
    FreeZoneOrFreeWarehouse,

    [EnumMember(Value = "ShipSupplier")]
    ShipSupplier,

    [EnumMember(Value = "Ship")]
    Ship,
}
