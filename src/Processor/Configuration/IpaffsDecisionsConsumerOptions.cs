using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class IpaffsDecisionsConsumerOptions
{
    public const string SectionName = "IpaffsDecisionsConsumer";

    [Required]
    public required bool AutoStartConsumers { get; init; }

    [Required]
    public required string QueueName { get; init; }

    public int ConsumersPerHost { get; init; } = 20;
}
