#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class CatchCertificates
{
    /// <summary>
    /// The catch certificate number
    /// </summary>
    [JsonPropertyName("certificateNumber")]
    public string? CertificateNumber { get; set; }

    /// <summary>
    /// The catch certificate weight number
    /// </summary>
    [JsonPropertyName("weight")]
    public double? Weight { get; set; }
}
