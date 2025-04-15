using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Part 1 - Holds the information related to veterinary checks and details
/// </summary>
public class VeterinaryInformation
{
    /// <summary>
    ///     External reference of approved establishments, which relates to a downstream service
    /// </summary>
    [JsonPropertyName("establishmentsOfOriginExternalReference")]
    public ExternalReference? EstablishmentsOfOriginExternalReference { get; set; }

    /// <summary>
    ///     List of establishments which were approved by UK to issue veterinary documents
    /// </summary>
    [JsonPropertyName("establishmentsOfOrigin")]
    public ApprovedEstablishment[]? EstablishmentsOfOrigins { get; set; }

    /// <summary>
    ///     Veterinary document identification
    /// </summary>
    [JsonPropertyName("veterinaryDocument")]
    public string? VeterinaryDocument { get; set; }

    /// <summary>
    ///     Veterinary document issue date
    /// </summary>
    [JsonPropertyName("veterinaryDocumentIssueDate")]
    public DateOnly? VeterinaryDocumentIssueDate { get; set; }

    /// <summary>
    ///     Additional documents
    /// </summary>
    [JsonPropertyName("accompanyingDocumentNumbers")]
    public string[]? AccompanyingDocumentNumbers { get; set; }

    /// <summary>
    ///     Accompanying documents
    /// </summary>
    [JsonPropertyName("accompanyingDocuments")]
    public AccompanyingDocument[]? AccompanyingDocuments { get; set; }

    /// <summary>
    ///     Catch certificate attachments
    /// </summary>
    [JsonPropertyName("catchCertificateAttachments")]
    public CatchCertificateAttachment[]? CatchCertificateAttachments { get; set; }

    /// <summary>
    ///     Details helpful for identification
    /// </summary>
    [JsonPropertyName("identificationDetails")]
    public IdentificationDetails[]? IdentificationDetails { get; set; }
}
