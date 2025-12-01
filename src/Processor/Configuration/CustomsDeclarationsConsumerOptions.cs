using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class CustomsDeclarationsConsumerOptions
{
    public const string SectionName = "CustomsDeclarationsConsumer";

    [Required]
    public required bool AutoStartConsumers { get; init; }

    [Required]
    public required string QueueName { get; init; }

    [Required]
    public required string DeadLetterQueueName { get; init; }

    public int ConsumersPerHost { get; init; } = 20;
}
