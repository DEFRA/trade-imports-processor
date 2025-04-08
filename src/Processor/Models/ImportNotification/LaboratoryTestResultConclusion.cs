using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum LaboratoryTestResultConclusion
{
    [EnumMember(Value = "Satisfactory")]
    Satisfactory,

    [EnumMember(Value = "Not satisfactory")]
    NotSatisfactory,

    [EnumMember(Value = "Not interpretable")]
    NotInterpretable,

    [EnumMember(Value = "Pending")]
    Pending,
}
