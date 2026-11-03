using System.Text.Json.Serialization;
using DataApiIuu = Defra.TradeImportsDataApi.Domain.Iuu;

namespace Defra.TradeImportsProcessor.Processor.Models.Iuu;

public class IuuNotification
{
    // We don't know the format of the IUU message yet. The following have been inferred from the Data Dictionary

    [JsonPropertyName("reference")]
    public required string Reference { get; set; }

    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }

    [JsonPropertyName("iuuCheckOutcome")]
    public required string IuuCheckOutcome { get; set; }

    public static explicit operator DataApiIuu.IuuNotification(IuuNotification iuuNotification)
    {
        return new DataApiIuu.IuuNotification
        {
            Reference = iuuNotification.Reference,
            LastUpdated = iuuNotification.LastUpdated,
            IuuOption = MapIuuOption(iuuNotification.Reference, iuuNotification.IuuCheckOutcome),
        };
    }

    private static string? MapIuuOption(string iuuNotificationReference, string iuuCheckOutcome)
    {
        // These will need updating when we know what the IUU Notification message will contain
        return iuuCheckOutcome switch
        {
            "IUUOK" => "IUUOK",
            "IUUNA" => "IUUNA",
            "IUUNotCompliant" => "IUUNotCompliant",
            _ => throw new ArgumentOutOfRangeException(
                $"IUU Notification {iuuNotificationReference} contains an unexpected IUU Check Outcome {iuuCheckOutcome}."
            ),
        };
    }
}
