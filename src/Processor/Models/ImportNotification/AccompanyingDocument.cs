#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Utils.Json;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Accompanying document
/// </summary>
public partial class AccompanyingDocument
{
    /// <summary>
    /// Additional document type
    /// </summary>
    [JsonPropertyName("documentType")]
    public AccompanyingDocumentDocumentType? DocumentType { get; set; }

    /// <summary>
    /// Additional document reference
    /// </summary>
    [JsonPropertyName("documentReference")]
    public string? DocumentReference { get; set; }

    /// <summary>
    /// Additional document issue date
    /// </summary>
    [JsonPropertyName("documentIssueDate")]
    [FlexibleDateOnlyJsonConverter]
    public DateOnly? DocumentIssueDate { get; set; }

    /// <summary>
    /// The UUID used for the uploaded file in blob storage
    /// </summary>
    [JsonPropertyName("attachmentId")]
    public string? AttachmentId { get; set; }

    /// <summary>
    /// The original filename of the uploaded file
    /// </summary>
    [JsonPropertyName("attachmentFilename")]
    public string? AttachmentFilename { get; set; }

    /// <summary>
    /// The MIME type of the uploaded file
    /// </summary>
    [JsonPropertyName("attachmentContentType")]
    public string? AttachmentContentType { get; set; }

    /// <summary>
    /// The UUID for the user that uploaded the file
    /// </summary>
    [JsonPropertyName("uploadUserId")]
    public string? UploadUserId { get; set; }

    /// <summary>
    /// The UUID for the organisation that the upload user is associated with
    /// </summary>
    [JsonPropertyName("uploadOrganisationId")]
    public string? UploadOrganisationId { get; set; }

    /// <summary>
    /// External reference of accompanying document, which relates to a downstream service
    /// </summary>
    [JsonPropertyName("externalReference")]
    public ExternalReference? ExternalReference { get; set; }
}
