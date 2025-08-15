using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class ClearanceRequestHeader
{
    [JsonPropertyName("entryReference")]
    public required string EntryReference { get; init; }

    [JsonPropertyName("entryVersionNumber")]
    public int? EntryVersionNumber { get; init; }

    [JsonPropertyName("previousVersionNumber")]
    public int? PreviousVersionNumber { get; set; }

    [JsonPropertyName("declarationUCR")]
    public string? DeclarationUcr { get; set; }

    [JsonPropertyName("declarationPartNumber")]
    public string? DeclarationPartNumber { get; set; }

    [JsonPropertyName("declarationType")]
    public string? DeclarationType { get; set; }

    [JsonPropertyName("arrivalDateTime")]
    public string? ArrivalDateTime { get; set; }

    [JsonPropertyName("submitterTURN")]
    public string? SubmitterTurn { get; set; }

    [JsonPropertyName("declarantId")]
    public string? DeclarantId { get; set; }

    [JsonPropertyName("declarantName")]
    public string? DeclarantName { get; set; }

    [JsonPropertyName("dispatchCountryCode")]
    public string? DispatchCountryCode { get; set; }

    [JsonPropertyName("goodsLocationCode")]
    public string? GoodsLocationCode { get; set; }

    [JsonPropertyName("masterUCR")]
    public string? MasterUcr { get; set; }
}
