using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class DecisionNotification(string mrn, ClearanceDecision clearanceDecision)
{
    [JsonPropertyName("serviceHeader")]
    public ServiceHeader ServiceHeader { get; set; } =
        new()
        {
            SourceSystem = "ALVS",
            DestinationSystem = "CDS",
            CorrelationId = clearanceDecision.CorrelationId!,
            ServiceCallTimestamp = clearanceDecision.Created,
        };

    [JsonPropertyName("header")]
    public Header Header { get; set; } =
        new()
        {
            EntryReference = mrn,
            EntryVersionNumber = clearanceDecision.ExternalVersionNumber,
            DecisionNumber = clearanceDecision.DecisionNumber,
        };

    [JsonPropertyName("items")]
    public Item[] Items { get; set; } = MapItems(clearanceDecision.Items);

    private static Item[] MapItems(ClearanceDecisionItem[] items)
    {
        if (items.Length == 0)
            return [];

        return items
            .Select(item => new Item
            {
                ItemNumber = item.ItemNumber,
                Checks = item
                    .Checks.Select(check => new Check
                    {
                        CheckCode = check.CheckCode,
                        DecisionCode = check.DecisionCode,
                        DecisionValidUntil = check.DecisionsValidUntil?.ToString("yyyyMMddHHmm"),
                        DecisionReason = check.DecisionReasons,
                    })
                    .ToArray(),
            })
            .ToArray();
    }
}
