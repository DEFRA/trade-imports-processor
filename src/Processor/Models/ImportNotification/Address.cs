using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Inspector Address
/// </summary>
public class Address
{
    /// <summary>
    ///     Street
    /// </summary>
    [JsonPropertyName("street")]
    public string? Street { get; set; }

    /// <summary>
    ///     City
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    ///     Country
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    ///     Postal Code
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>
    ///     1st line of address
    /// </summary>
    [JsonPropertyName("addressLine1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    ///     2nd line of address
    /// </summary>
    [JsonPropertyName("addressLine2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    ///     3rd line of address
    /// </summary>
    [JsonPropertyName("addressLine3")]
    public string? AddressLine3 { get; set; }

    /// <summary>
    ///     Post / zip code
    /// </summary>
    [JsonPropertyName("postalZipCode")]
    public string? PostalZipCode { get; set; }

    /// <summary>
    ///     country 2-digits ISO code
    /// </summary>
    [JsonPropertyName("countryISOCode")]
    public string? CountryIsoCode { get; set; }

    /// <summary>
    ///     Email address
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    ///     UK phone number
    /// </summary>
    [JsonPropertyName("ukTelephone")]
    public string? UkTelephone { get; set; }

    /// <summary>
    ///     Telephone number
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>
    ///     International phone number
    /// </summary>
    [JsonPropertyName("internationalTelephone")]
    public InternationalTelephone? InternationalTelephone { get; set; }
}
