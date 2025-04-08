using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PurposeInternalMarketPurpose
{
    [EnumMember(Value = "Animal Feeding Stuff")]
    AnimalFeedingStuff,

    [EnumMember(Value = "Human Consumption")]
    HumanConsumption,

    [EnumMember(Value = "Pharmaceutical Use")]
    PharmaceuticalUse,

    [EnumMember(Value = "Technical Use")]
    TechnicalUse,

    [EnumMember(Value = "Other")]
    Other,

    [EnumMember(Value = "Commercial Sale")]
    CommercialSale,

    [EnumMember(Value = "Commercial sale or change of ownership")]
    CommercialSaleOrChangeOfOwnership,

    [EnumMember(Value = "Rescue")]
    Rescue,

    [EnumMember(Value = "Breeding")]
    Breeding,

    [EnumMember(Value = "Research")]
    Research,

    [EnumMember(Value = "Racing or Competition")]
    RacingOrCompetition,

    [EnumMember(Value = "Approved Premises or Body")]
    ApprovedPremisesOrBody,

    [EnumMember(Value = "Companion Animal not for Resale or Rehoming")]
    CompanionAnimalNotForResaleOrRehoming,

    [EnumMember(Value = "Production")]
    Production,

    [EnumMember(Value = "Slaughter")]
    Slaughter,

    [EnumMember(Value = "Fattening")]
    Fattening,

    [EnumMember(Value = "Game Restocking")]
    GameRestocking,

    [EnumMember(Value = "Registered Horses")]
    RegisteredHorses,
}
