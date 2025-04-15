using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Catch certificate details for uploaded attachment
/// </summary>
public class CatchCertificateDetails
{
    /// <summary>
    ///     The UUID of the catch certificate
    /// </summary>
    [JsonPropertyName("catchCertificateId")]
    public string? CatchCertificateId { get; set; }

    /// <summary>
    ///     Catch certificate reference
    /// </summary>
    [JsonPropertyName("catchCertificateReference")]
    public string? CatchCertificateReference { get; set; }

    /// <summary>
    ///     Catch certificate date of issue
    /// </summary>
    [JsonPropertyName("dateOfIssue")]
    public DateOnly? DateOfIssue { get; set; }

    /// <summary>
    ///     Catch certificate flag state of catching vessel(s)
    /// </summary>
    [JsonPropertyName("flagState")]
    public string? FlagState { get; set; }

    /// <summary>
    ///     List of species imported under this catch certificate
    /// </summary>
    [JsonPropertyName("species")]
    public string[]? Species { get; set; }
}
