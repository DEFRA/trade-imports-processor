using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class CustomsDeclarationsConsumerOptions
{
    public const string SectionName = "CustomsDeclarationsConsumer";

    [Required]
    public required string QueueName { get; init; }
}
