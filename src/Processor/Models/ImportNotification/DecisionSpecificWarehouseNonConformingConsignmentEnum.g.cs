
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionSpecificWarehouseNonConformingConsignmentEnum
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