
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum AnimalCertificationEnum
{

    [EnumMember(Value = "Animal feeding stuff")]
    AnimalFeedingStuff,

    [EnumMember(Value = "Approved")]
    Approved,

    [EnumMember(Value = "Artificial reproduction")]
    ArtificialReproduction,

    [EnumMember(Value = "Breeding")]
    Breeding,

    [EnumMember(Value = "Circus")]
    Circus,

    [EnumMember(Value = "Commercial sale")]
    CommercialSale,

    [EnumMember(Value = "Commercial sale or change of ownership")]
    CommercialSaleOrChangeOfOwnership,

    [EnumMember(Value = "Fattening")]
    Fattening,

    [EnumMember(Value = "Game restocking")]
    GameRestocking,

    [EnumMember(Value = "Human consumption")]
    HumanConsumption,

    [EnumMember(Value = "Internal market")]
    InternalMarket,

    [EnumMember(Value = "Other")]
    Other,

    [EnumMember(Value = "Personally owned pets not for rehoming")]
    PersonallyOwnedPetsNotForRehoming,

    [EnumMember(Value = "Pets")]
    Pets,

    [EnumMember(Value = "Production")]
    Production,

    [EnumMember(Value = "Quarantine")]
    Quarantine,

    [EnumMember(Value = "Racing/Competition")]
    RacingCompetition,

    [EnumMember(Value = "Registered equidae")]
    RegisteredEquidae,

    [EnumMember(Value = "Registered")]
    Registered,

    [EnumMember(Value = "Rejected or Returned consignment")]
    RejectedOrReturnedConsignment,

    [EnumMember(Value = "Relaying")]
    Relaying,

    [EnumMember(Value = "Rescue/Rehoming")]
    RescueRehoming,

    [EnumMember(Value = "Research")]
    Research,

    [EnumMember(Value = "Slaughter")]
    Slaughter,

    [EnumMember(Value = "Technical/Pharmaceutical use")]
    TechnicalPharmaceuticalUse,

    [EnumMember(Value = "Transit")]
    Transit,

    [EnumMember(Value = "Zoo/Collection")]
    ZooCollection,

}