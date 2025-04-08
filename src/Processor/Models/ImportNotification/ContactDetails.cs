#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Person to be contacted if there is an issue with the consignment
/// </summary>
public partial class ContactDetails
{
    /// <summary>
    /// Name of designated contact
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Telephone number of designated contact
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>
    /// Email address of designated contact
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Name of agent representing designated contact
    /// </summary>
    [JsonPropertyName("agent")]
    public string? Agent { get; set; }
}
