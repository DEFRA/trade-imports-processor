#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// inspector
/// </summary>
public partial class Inspector
{
    /// <summary>
    /// Name of inspector
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Phone number of inspector
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Email address of inspector
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
