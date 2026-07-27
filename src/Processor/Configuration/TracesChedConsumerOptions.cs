using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class TracesChedConsumerOptions
{
    public const string SectionName = "TracesChedsConsumer";

    [Required]
    public required bool AutoStartConsumers { get; init; }

    [Required]
    public required string QueueName { get; init; }

    public string DeadLetterQueueName => $"{QueueName}-deadletter";

    public int ConsumersPerHost { get; init; } = 20;
}
