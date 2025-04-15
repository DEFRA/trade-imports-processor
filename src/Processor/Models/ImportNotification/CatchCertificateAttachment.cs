using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Catch certificate attachment
/// </summary>
public class CatchCertificateAttachment
{
    /// <summary>
    ///     The UUID of the uploaded catch certificate file in blob storage
    /// </summary>
    [JsonPropertyName("attachmentId")]
    public string? AttachmentId { get; set; }

    /// <summary>
    ///     The total number of catch certificates on the attachment
    /// </summary>
    [JsonPropertyName("numberOfCatchCertificates")]
    public int? NumberOfCatchCertificates { get; set; }

    /// <summary>
    ///     List of catch certificate details
    /// </summary>
    [JsonPropertyName("CatchCertificateDetails")]
    public CatchCertificateDetails[]? CatchCertificateDetails { get; set; }
}
