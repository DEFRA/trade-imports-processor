using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionFreeCirculationPurpose
{
    [EnumMember(Value = "Animal Feeding Stuff")]
    AnimalFeedingStuff,

    [EnumMember(Value = "Human Consumption")]
    HumanConsumption,

    [EnumMember(Value = "Pharmaceutical Use")]
    PharmaceuticalUse,

    [EnumMember(Value = "Technical Use")]
    TechnicalUse,

    [EnumMember(Value = "Further Process")]
    FurtherProcess,

    [EnumMember(Value = "Other")]
    Other,
}
