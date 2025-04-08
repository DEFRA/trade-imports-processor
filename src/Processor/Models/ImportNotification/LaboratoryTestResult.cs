#nullable enable

using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
/// Tests results corresponding to LaboratoryTests
/// </summary>
public partial class LaboratoryTestResult
{
    /// <summary>
    /// When sample was used
    /// </summary>
    [JsonPropertyName("sampleUseByDate")]
    public string? SampleUseByDate { get; set; }

    /// <summary>
    /// When it was released
    /// </summary>
    [JsonPropertyName("releasedDate")]
    public DateOnly? ReleasedDate { get; set; }

    /// <summary>
    /// Laboratory test method
    /// </summary>
    [JsonPropertyName("laboratoryTestMethod")]
    public string? LaboratoryTestMethod { get; set; }

    /// <summary>
    /// Result of test
    /// </summary>
    [JsonPropertyName("results")]
    public string? Results { get; set; }

    /// <summary>
    /// Conclusion of laboratory test
    /// </summary>
    [JsonPropertyName("conclusion")]
    public LaboratoryTestResultConclusion? Conclusion { get; set; }

    /// <summary>
    /// Date of lab test created in IPAFFS
    /// </summary>
    [JsonPropertyName("labTestCreatedDate")]
    public DateOnly? LabTestCreatedDate { get; set; }
}
