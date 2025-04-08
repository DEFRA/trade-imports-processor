#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class IdentificationDetails
{
    /// <summary>
    /// Identification detail
    /// </summary>
    [JsonPropertyName("identificationDetail")]
    public string? IdentificationDetail { get; set; }

    /// <summary>
    /// Identification description
    /// </summary>
    [JsonPropertyName("identificationDescription")]
    public string? IdentificationDescription { get; set; }
}
