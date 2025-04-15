using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Approved Establishment details
/// </summary>
public class ApprovedEstablishment
{
    /// <summary>
    ///     ID
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Name of approved establishment
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Country of approved establishment
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    ///     Types of approved establishment
    /// </summary>
    [JsonPropertyName("types")]
    public string[]? Types { get; set; }

    /// <summary>
    ///     Approval number
    /// </summary>
    [JsonPropertyName("approvalNumber")]
    public string? ApprovalNumber { get; set; }

    /// <summary>
    ///     Section of approved establishment
    /// </summary>
    [JsonPropertyName("section")]
    public string? Section { get; set; }
}
