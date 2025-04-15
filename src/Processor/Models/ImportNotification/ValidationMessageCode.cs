using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Validation field code-message representation
/// </summary>
public class ValidationMessageCode
{
    /// <summary>
    ///     Field
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }

    /// <summary>
    ///     Code
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
