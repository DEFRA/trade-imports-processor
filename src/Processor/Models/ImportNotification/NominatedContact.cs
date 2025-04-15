using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Person to be nominated for text and email contact for the consignment
/// </summary>
public class NominatedContact
{
    /// <summary>
    ///     Name of nominated contact
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Email address of nominated contact
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    ///     Telephone number of nominated contact
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }
}
