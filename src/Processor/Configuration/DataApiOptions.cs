using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class DataApiOptions
{
    public const string SectionName = "DataApi";

    [Required]
    public required string BaseAddress { get; init; }
}
