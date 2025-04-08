using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum LaboratoryTestsTestReason
{
    [EnumMember(Value = "Random")]
    Random,

    [EnumMember(Value = "Suspicious")]
    Suspicious,

    [EnumMember(Value = "Re-enforced")]
    ReEnforced,

    [EnumMember(Value = "Intensified controls")]
    IntensifiedControls,

    [EnumMember(Value = "Required")]
    Required,

    [EnumMember(Value = "Latent infection sampling")]
    LatentInfectionSampling,
}
