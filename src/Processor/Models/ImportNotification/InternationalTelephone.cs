#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// International phone number
/// </summary>
public partial class InternationalTelephone
{
    /// <summary>
    /// Country code of phone number
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [JsonPropertyName("subscriberNumber")]
    public string? SubscriberNumber { get; set; }
}
