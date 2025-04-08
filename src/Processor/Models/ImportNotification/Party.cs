#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Party details
/// </summary>
public partial class Party
{
    /// <summary>
    /// IPAFFS ID of party
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Name of party
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Company ID
    /// </summary>
    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Contact ID (B2C)
    /// </summary>
    [JsonPropertyName("contactId")]
    public string? ContactId { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Addresses
    /// </summary>
    [JsonPropertyName("address")]
    public string[]? Addresses { get; set; }

    /// <summary>
    /// County
    /// </summary>
    [JsonPropertyName("county")]
    public string? County { get; set; }

    /// <summary>
    /// Post code of party
    /// </summary>
    [JsonPropertyName("postCode")]
    public string? PostCode { get; set; }

    /// <summary>
    /// Country of party
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// City
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// TRACES ID
    /// </summary>
    [JsonPropertyName("tracesID")]
    public int? TracesId { get; set; }

    /// <summary>
    /// Type of party
    /// </summary>
    [JsonPropertyName("type")]
    public PartyType? Type { get; set; }

    /// <summary>
    /// Approval number
    /// </summary>
    [JsonPropertyName("approvalNumber")]
    public string? ApprovalNumber { get; set; }

    /// <summary>
    /// Phone number of party
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Fax number of party
    /// </summary>
    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    /// <summary>
    /// Email number of party
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
