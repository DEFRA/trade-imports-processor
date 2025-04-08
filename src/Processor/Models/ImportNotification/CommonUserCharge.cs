#nullable enable

using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class CommonUserCharge
{
    /// <summary>
    /// Indicates whether the last applicable change was successfully send over the interface to Trade Charge
    /// </summary>
    [JsonPropertyName("wasSentToTradeCharge")]
    public bool? WasSentToTradeCharge { get; set; }
}
