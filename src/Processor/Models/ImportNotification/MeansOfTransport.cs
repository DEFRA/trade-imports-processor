using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Details of transport
/// </summary>
public class MeansOfTransport
{
    /// <summary>
    ///     Type of transport
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Document for transport
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }

    /// <summary>
    ///     ID of transport
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
