#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Result of risk assessment by the risk scorer
/// </summary>
public partial class RiskAssessmentResult
{
    /// <summary>
    /// List of risk assessed commodities
    /// </summary>
    [JsonPropertyName("commodityResults")]
    public CommodityRiskResult[]? CommodityResults { get; set; }

    /// <summary>
    /// Date and time of assessment
    /// </summary>
    [JsonPropertyName("assessmentDateTime")]
    [UnknownTimeZoneDateTimeJsonConverter(nameof(AssessmentDateTime))]
    public DateTime? AssessmentDateTime { get; set; }
}
