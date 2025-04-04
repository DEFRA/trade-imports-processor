using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    [Required]
    public required ServiceBusSubscriptionOptions Notifications { get; init; }
}

public class ServiceBusSubscriptionOptions
{
    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string Topic { get; init; }

    [Required]
    public required string Subscription { get; init; }
}
