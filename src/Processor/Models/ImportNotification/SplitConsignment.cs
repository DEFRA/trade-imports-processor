#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Present if the consignment has been split
/// </summary>
public partial class SplitConsignment
{
    /// <summary>
    /// Reference number of the valid split consignment
    /// </summary>
    [JsonPropertyName("validReferenceNumber")]
    public string? ValidReferenceNumber { get; set; }

    /// <summary>
    /// Reference number of the rejected split consignment
    /// </summary>
    [JsonPropertyName("rejectedReferenceNumber")]
    public string? RejectedReferenceNumber { get; set; }
}
