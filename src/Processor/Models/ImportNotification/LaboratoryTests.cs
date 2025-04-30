using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Laboratory tests details
/// </summary>
public class LaboratoryTests
{
    /// <summary>
    ///     Date of tests
    /// </summary>
    [JsonPropertyName("testDate")]
    [UnknownTimeZoneDateTimeJsonConverter(nameof(TestDate))]
    public DateTime? TestDate { get; set; }

    /// <summary>
    ///     Reason for test
    /// </summary>
    [JsonPropertyName("testReason")]
    public string? TestReason { get; set; }

    /// <summary>
    ///     List of details of individual tests performed or to be performed
    /// </summary>
    [JsonPropertyName("singleLaboratoryTests")]
    public SingleLaboratoryTest[]? SingleLaboratoryTests { get; set; }
}
