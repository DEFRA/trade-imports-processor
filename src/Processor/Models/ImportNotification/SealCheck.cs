using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Details of the seal check
/// </summary>
public class SealCheck
{
    /// <summary>
    ///     Is seal check satisfactory
    /// </summary>
    [JsonPropertyName("satisfactory")]
    public bool? Satisfactory { get; set; }

    /// <summary>
    ///     reason for not satisfactory
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    ///     Official inspector
    /// </summary>
    [JsonPropertyName("officialInspector")]
    public OfficialInspector? OfficialInspector { get; set; }

    /// <summary>
    ///     date and time of seal check
    /// </summary>
    [JsonPropertyName("dateTimeOfCheck")]
    [UnknownTimeZoneDateTimeJsonConverter(nameof(DateTimeOfCheck))]
    public DateTime? DateTimeOfCheck { get; set; }
}
