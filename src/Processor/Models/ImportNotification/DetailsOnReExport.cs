using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Details on re-export
/// </summary>
public class DetailsOnReExport
{
    /// <summary>
    ///     Date of re-export
    /// </summary>
    [JsonPropertyName("date")]
    public DateOnly? Date { get; set; }

    /// <summary>
    ///     Number of vehicle
    /// </summary>
    [JsonPropertyName("meansOfTransportNo")]
    public string? MeansOfTransportNo { get; set; }

    /// <summary>
    ///     Type of transport to be used
    /// </summary>
    [JsonPropertyName("transportType")]
    public string? TransportType { get; set; }

    /// <summary>
    ///     Document issued for re-export
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }

    /// <summary>
    ///     Two letter ISO code for country of re-dispatching
    /// </summary>
    [JsonPropertyName("countryOfReDispatching")]
    public string? CountryOfReDispatching { get; set; }

    /// <summary>
    ///     Exit BIP (where consignment will leave the country)
    /// </summary>
    [JsonPropertyName("exitBIP")]
    public string? ExitBip { get; set; }
}
