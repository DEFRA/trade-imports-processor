using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class IuuNotificationConsumerOptions
{
    public const string SectionName = "IuuNotificationsConsumer";

    [Required]
    public required bool AutoStartConsumers { get; init; }

    [Required]
    public required string QueueName { get; init; }

    public string DeadLetterQueueName => $"{QueueName}-deadletter";

    public int ConsumersPerHost { get; init; } = 20;
}
