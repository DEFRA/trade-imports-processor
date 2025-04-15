using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Information about logged-in user
/// </summary>
public class UserInformation
{
    /// <summary>
    ///     Display name
    /// </summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    ///     User ID
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     Is this user a control
    /// </summary>
    [JsonPropertyName("isControlUser")]
    public bool? IsControlUser { get; set; }
}
