
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum DecisionDecisionEnum
{

    [EnumMember(Value = "Non Acceptable")]
    NonAcceptable,

    [EnumMember(Value = "Acceptable for Internal Market")]
    AcceptableForInternalMarket,

    [EnumMember(Value = "Acceptable if Channeled")]
    AcceptableIfChanneled,

    [EnumMember(Value = "Acceptable for Transhipment")]
    AcceptableForTranshipment,

    [EnumMember(Value = "Acceptable for Transit")]
    AcceptableForTransit,

    [EnumMember(Value = "Acceptable for Temporary Import")]
    AcceptableForTemporaryImport,

    [EnumMember(Value = "Acceptable for Specific Warehouse")]
    AcceptableForSpecificWarehouse,

    [EnumMember(Value = "Acceptable for Private Import")]
    AcceptableForPrivateImport,

    [EnumMember(Value = "Acceptable for Transfer")]
    AcceptableForTransfer,

    [EnumMember(Value = "Horse Re-entry")]
    HorseReEntry,

}