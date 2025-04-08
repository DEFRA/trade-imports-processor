#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Details of transport
/// </summary>
public partial class MeansOfTransport
{
    /// <summary>
    /// Type of transport
    /// </summary>
    [JsonPropertyName("type")]
    public MeansOfTransportType? Type { get; set; }

    /// <summary>
    /// Document for transport
    /// </summary>
    [JsonPropertyName("document")]
    public string? Document { get; set; }

    /// <summary>
    /// ID of transport
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
