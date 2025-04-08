#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Contains countries and transfer points that consignment is going through
/// </summary>
public partial class Route
{
    [JsonPropertyName("transitingStates")]
    public string[]? TransitingStates { get; set; }
}
