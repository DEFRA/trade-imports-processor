using System.ComponentModel.DataAnnotations;

namespace Defra.TradeImportsProcessor.Processor.Configuration;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    [Required]
    public required ServiceBusSubscriptionOptions Gmrs { get; init; }

    [Required]
    public required ServiceBusSubscriptionOptions Notifications { get; init; }

    [Required]
    public required ServiceBusOptionsBase Ipaffs { get; init; }
}

public class ServiceBusOptionsBase
{
    [Required]
    public required string Topic { get; init; }

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string Subscription { get; init; }
}

public class ServiceBusSubscriptionOptions : ServiceBusOptionsBase
{
    [Required]
    public required bool AutoStartConsumers { get; init; }

    public int ConsumersPerHost { get; init; } = 20;
}
