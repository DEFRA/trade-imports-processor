
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DetailsOnReExportTransportTypeEnum
{

    [EnumMember(Value = "rail")]
    Rail,

    [EnumMember(Value = "plane")]
    Plane,

    [EnumMember(Value = "ship")]
    Ship,

    [EnumMember(Value = "road")]
    Road,

    [EnumMember(Value = "other")]
    Other,

    [EnumMember(Value = "c_ship_road")]
    CShipRoad,

    [EnumMember(Value = "c_ship_rail")]
    CShipRail,

}