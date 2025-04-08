
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum MeansOfTransportTypeEnum
{

    [EnumMember(Value = "Aeroplane")]
    Aeroplane,

    [EnumMember(Value = "Road Vehicle")]
    RoadVehicle,

    [EnumMember(Value = "Railway Wagon")]
    RailwayWagon,

    [EnumMember(Value = "Ship")]
    Ship,

    [EnumMember(Value = "Other")]
    Other,

    [EnumMember(Value = "Road vehicle Aeroplane")]
    RoadVehicleAeroplane,

    [EnumMember(Value = "Ship Railway wagon")]
    ShipRailwayWagon,

    [EnumMember(Value = "Ship Road vehicle")]
    ShipRoadVehicle,

}