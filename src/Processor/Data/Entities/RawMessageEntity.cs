using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Data.Extensions;

namespace Defra.TradeImportsProcessor.Processor.Data.Entities;

[DbCollection("RawMessage")]
public class RawMessageEntity : IDataEntity
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("etag")]
    public string ETag { get; set; } = null!;

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("updated")]
    public DateTime Updated { get; set; }

    [JsonPropertyName("resourceId")]
    public required string ResourceId { get; set; }

    [JsonPropertyName("resourceType")]
    public required string ResourceType { get; set; }

    [JsonPropertyName("headers")]
    public Dictionary<string, string?> Headers { get; set; } = [];

    [JsonPropertyName("messageId")]
    public required string MessageId { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }

    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    public void OnSave() { }
}
