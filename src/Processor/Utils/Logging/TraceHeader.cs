using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Utils.Logging;

public class TraceHeader
{
    [ConfigurationKeyName("TraceHeader")]
    [Required]
    public required string Name { get; set; }
}
