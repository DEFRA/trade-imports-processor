#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Details about the manual inspection override
/// </summary>
public partial class InspectionOverride
{
    /// <summary>
    /// Original inspection decision
    /// </summary>
    [JsonPropertyName("originalDecision")]
    public string? OriginalDecision { get; set; }

    /// <summary>
    /// The time the risk decision is overridden
    /// </summary>
    [JsonPropertyName("overriddenOn")]
    public DateTime? OverriddenOn { get; set; }

    /// <summary>
    /// User entity who has manually overridden the inspection
    /// </summary>
    [JsonPropertyName("overriddenBy")]
    public UserInformation? OverriddenBy { get; set; }
}
