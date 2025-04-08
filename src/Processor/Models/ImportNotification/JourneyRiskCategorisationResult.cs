#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Details of the risk categorisation level for a notification
/// </summary>
public partial class JourneyRiskCategorisationResult
{
    /// <summary>
    /// Risk Level is defined using enum values High,Medium,Low
    /// </summary>
    [JsonPropertyName("riskLevel")]
    public JourneyRiskCategorisationResultRiskLevel? RiskLevel { get; set; }

    /// <summary>
    /// Indicator of whether the risk level was determined by the system or by the user
    /// </summary>
    [JsonPropertyName("riskLevelMethod")]
    public JourneyRiskCategorisationResultRiskLevelMethod? RiskLevelMethod { get; set; }

    /// <summary>
    /// The date and time the risk level has been set for a notification
    /// </summary>
    [JsonPropertyName("riskLevelDateTime")]
    [UnknownTimeZoneDateTimeJsonConverter(nameof(RiskLevelSetFor))]
    public DateTime? RiskLevelSetFor { get; set; }
}
