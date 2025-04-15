using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public class BillingInformation
{
    /// <summary>
    ///     Indicates whether user has confirmed their billing information
    /// </summary>
    [JsonPropertyName("isConfirmed")]
    public bool? IsConfirmed { get; set; }

    /// <summary>
    ///     Billing email address
    /// </summary>
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    /// <summary>
    ///     Billing phone number
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    ///     Billing Contact Name
    /// </summary>
    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }

    /// <summary>
    ///     Billing postal address
    /// </summary>
    [JsonPropertyName("postalAddress")]
    public PostalAddress? PostalAddress { get; set; }
}
