#nullable enable

using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public partial class ComplementParameterSet
{
    /// <summary>
    /// UUID used to match commodityComplement to its complementParameter set. CHEDPP only
    /// </summary>
    [JsonPropertyName("uniqueComplementID")]
    public string? UniqueComplementId { get; set; }

    [JsonPropertyName("complementID")]
    public int? ComplementId { get; set; }

    [JsonPropertyName("speciesID")]
    public string? SpeciesId { get; set; }

    [JsonPropertyName("keyDataPair")]
    [JsonConverter(typeof(KeyDataPairsToDictionaryStringObjectJsonConverter))]
    public IDictionary<string, object>? KeyDataPairs { get; set; }

    /// <summary>
    /// Catch certificate details
    /// </summary>
    [JsonPropertyName("catchCertificates")]
    public CatchCertificates[]? CatchCertificates { get; set; }

    /// <summary>
    /// Data used to identify the complements inside an IMP consignment
    /// </summary>
    [JsonPropertyName("identifiers")]
    public Identifiers[]? Identifiers { get; set; }
}
