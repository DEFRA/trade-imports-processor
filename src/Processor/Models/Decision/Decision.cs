using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;

namespace Defra.TradeImportsProcessor.Processor.Models.Decision;

public class DecisionCheck
{
    [JsonPropertyName("checkCode")]
    public string? CheckCode { get; set; }

    [JsonPropertyName("departmentCode")]
    public string? DepartmentCode { get; set; }

    [JsonPropertyName("decisionCode")]
    public string? DecisionCode { get; set; }

    [JsonPropertyName("decisionsValidUntil")]
    public DateTime? DecisionsValidUntil { get; set; }

    [JsonPropertyName("decisionReasons")]
    public string[]? DecisionReasons { get; set; }
}

public class DecisionItems
{
    [JsonPropertyName("itemNumber")]
    public int ItemNumber { get; set; }

    [JsonPropertyName("documents")]
    public Document[]? Documents { get; set; }

    [JsonPropertyName("checks")]
    public DecisionCheck[]? Checks { get; set; }
}

/// <summary>
///     This is a copy of the AlvsClearanceRequest
///     As a temporary measure to allow us to distinguish between the two types when
///     selecting consumers via the DI container. We'll also start to
///     Plan is to follow the same approach as other types (currently generated) in time.
/// </summary>
public class Decision
{
    /// <summary>
    /// </summary>
    [JsonPropertyName("serviceHeader")]
    public ServiceHeader? ServiceHeader { get; set; }

    /// <summary>
    /// </summary>
    [JsonPropertyName("header")]
    public DecisionHeader? Header { get; set; }

    /// <summary>
    /// </summary>
    [JsonPropertyName("items")]
    public DecisionItems[]? Items { get; set; }
}
