//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable

using System.Text.Json.Serialization;
using System.Dynamic;


namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;


/// <summary>
/// Result of the risk assessment of a commodity
/// </summary>
public partial class CommodityRiskResult  //
{


    /// <summary>
    /// CHED-A, CHED-D, CHED-P - what is the commodity complement risk decision
    /// </summary>
    [JsonPropertyName("riskDecision")]
    public CommodityRiskResultRiskDecisionEnum? RiskDecision { get; set; }


    /// <summary>
    /// Transit CHED - what is the commodity complement exit risk decision
    /// </summary>
    [JsonPropertyName("exitRiskDecision")]
    public CommodityRiskResultExitRiskDecisionEnum? ExitRiskDecision { get; set; }


    /// <summary>
    /// HMI decision required
    /// </summary>
    [JsonPropertyName("hmiDecision")]
    public CommodityRiskResultHmiDecisionEnum? HmiDecision { get; set; }


    /// <summary>
    /// PHSI decision required
    /// </summary>
    [JsonPropertyName("phsiDecision")]
    public CommodityRiskResultPhsiDecisionEnum? PhsiDecision { get; set; }


    /// <summary>
    /// PHSI classification
    /// </summary>
    [JsonPropertyName("phsiClassification")]
    public CommodityRiskResultPhsiClassificationEnum? PhsiClassification { get; set; }


    /// <summary>
    /// PHSI Decision Breakdown
    /// </summary>
    [JsonPropertyName("phsi")]
    public Phsi? Phsi { get; set; }


    /// <summary>
    /// UUID used to match to the complement parameter set
    /// </summary>
    [JsonPropertyName("uniqueId")]
    public string? UniqueId { get; set; }


    /// <summary>
    /// EPPO Code for the species
    /// </summary>
    [JsonPropertyName("eppoCode")]
    public string? EppoCode { get; set; }


    /// <summary>
    /// Name or ID of the variety
    /// </summary>
    [JsonPropertyName("variety")]
    public string? Variety { get; set; }


    /// <summary>
    /// Whether or not a plant is woody
    /// </summary>
    [JsonPropertyName("isWoody")]
    public bool? IsWoody { get; set; }


    /// <summary>
    /// Indoor or Outdoor for a plant
    /// </summary>
    [JsonPropertyName("indoorOutdoor")]
    public string? IndoorOutdoor { get; set; }


    /// <summary>
    /// Whether the propagation is considered a Plant, Bulb, Seed or None
    /// </summary>
    [JsonPropertyName("propagation")]
    public string? Propagation { get; set; }


    /// <summary>
    /// Rule type for PHSI checks
    /// </summary>
    [JsonPropertyName("phsiRuleType")]
    public string? PhsiRuleType { get; set; }

}