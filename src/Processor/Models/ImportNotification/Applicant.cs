using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

/// <summary>
///     Laboratory tests information details and information about laboratory that did the test
/// </summary>
public class Applicant
{
    /// <summary>
    ///     Name of laboratory
    /// </summary>
    [JsonPropertyName("laboratory")]
    public string? Laboratory { get; set; }

    /// <summary>
    ///     Laboratory address
    /// </summary>
    [JsonPropertyName("laboratoryAddress")]
    public string? LaboratoryAddress { get; set; }

    /// <summary>
    ///     Laboratory identification
    /// </summary>
    [JsonPropertyName("laboratoryIdentification")]
    public string? LaboratoryIdentification { get; set; }

    /// <summary>
    ///     Laboratory phone number
    /// </summary>
    [JsonPropertyName("laboratoryPhoneNumber")]
    public string? LaboratoryPhoneNumber { get; set; }

    /// <summary>
    ///     Laboratory email
    /// </summary>
    [JsonPropertyName("laboratoryEmail")]
    public string? LaboratoryEmail { get; set; }

    /// <summary>
    ///     Sample batch number
    /// </summary>
    [JsonPropertyName("sampleBatchNumber")]
    public string? SampleBatchNumber { get; set; }

    /// <summary>
    ///     Type of analysis
    /// </summary>
    [JsonPropertyName("analysisType")]
    public ApplicantAnalysisType? AnalysisType { get; set; }

    /// <summary>
    ///     Number of samples analysed
    /// </summary>
    [JsonPropertyName("numberOfSamples")]
    public int? NumberOfSamples { get; set; }

    /// <summary>
    ///     Type of sample
    /// </summary>
    [JsonPropertyName("sampleType")]
    public string? SampleType { get; set; }

    /// <summary>
    ///     Conservation of sample
    /// </summary>
    [JsonPropertyName("conservationOfSample")]
    public ApplicantConservationOfSample? ConservationOfSample { get; set; }

    /// <summary>
    ///     inspector
    /// </summary>
    [JsonPropertyName("inspector")]
    public Inspector? Inspector { get; set; }

    /// <summary>
    ///     Date the sample is taken
    /// </summary>
    [JsonPropertyName("sampleDate")]
    public DateOnly? SampleDate { get; set; }

    /// <summary>
    ///     Time the sample is taken
    /// </summary>
    [JsonPropertyName("sampleTime")]
    public TimeOnly? SampleTime { get; set; }
}
