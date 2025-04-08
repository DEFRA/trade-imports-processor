using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum PurposeForImportOrAdmission
{
    [EnumMember(Value = "Definitive import")]
    DefinitiveImport,

    [EnumMember(Value = "Horses Re-entry")]
    HorsesReEntry,

    [EnumMember(Value = "Temporary admission horses")]
    TemporaryAdmissionHorses,
}
