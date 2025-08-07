using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class RawMessageLoggingOptions
{
    public const string SectionName = "RawMessageLogging";

    public bool Enabled { get; init; } = false;

    [Range(1, 90)]
    public int TtlDays { get; init; } = 30;
}
