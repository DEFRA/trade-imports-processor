using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Official veterinarian information
/// </summary>
public class OfficialVeterinarian
{
    /// <summary>
    ///     First name of official veterinarian
    /// </summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    /// <summary>
    ///     Last name of official veterinarian
    /// </summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    /// <summary>
    ///     Email address of official veterinarian
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    ///     Phone number of official veterinarian
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    ///     Fax number of official veterinarian
    /// </summary>
    [JsonPropertyName("fax")]
    public string? Fax { get; set; }

    /// <summary>
    ///     Date of sign
    /// </summary>
    [JsonPropertyName("signed")]
    public string? Signed { get; set; }
}
