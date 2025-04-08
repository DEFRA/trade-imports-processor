#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Details of Control (Part 3)
/// </summary>
public partial class Control
{
    /// <summary>
    /// Feedback information of Control
    /// </summary>
    [JsonPropertyName("feedbackInformation")]
    public FeedbackInformation? FeedbackInformation { get; set; }

    /// <summary>
    /// Details on re-export
    /// </summary>
    [JsonPropertyName("detailsOnReExport")]
    public DetailsOnReExport? DetailsOnReExport { get; set; }

    /// <summary>
    /// Official inspector
    /// </summary>
    [JsonPropertyName("officialInspector")]
    public OfficialInspector? OfficialInspector { get; set; }

    /// <summary>
    /// Is the consignment leaving UK borders?
    /// </summary>
    [JsonPropertyName("consignmentLeave")]
    public ControlConsignmentLeave? ConsignmentLeave { get; set; }
}
