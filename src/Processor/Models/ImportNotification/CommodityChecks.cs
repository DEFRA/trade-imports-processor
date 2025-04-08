#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class CommodityChecks
{
    /// <summary>
    /// UUID used to match the commodityChecks to the commodityComplement
    /// </summary>
    [JsonPropertyName("uniqueComplementId")]
    public string? UniqueComplementId { get; set; }

    [JsonPropertyName("checks")]
    public InspectionCheck[]? Checks { get; set; }

    /// <summary>
    /// Manually entered validity period, allowed if risk decision is INSPECTION_REQUIRED and HMI check status &#x27;Compliant&#x27; or &#x27;Not inspected&#x27;
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    public int? ValidityPeriod { get; set; }
}
