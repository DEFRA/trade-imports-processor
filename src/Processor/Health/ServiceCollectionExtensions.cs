using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Health;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddAsbTopic(
                "IPAFFS notifications",
                sp => sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Notifications,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            )
            .AddAsbTopic(
                "DMP GMRs",
                sp => sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Gmrs,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            )
            // Azure service bus emulator does not support the management client, which
            // this health check uses. Therefore, it will only work against real Azure
            .AddAsbTopic(
                "IPAFFS topic (outgoing)",
                sp => sp.GetRequiredService<IOptions<ServiceBusOptions>>().Value.Ipaffs,
                failureStatus: HealthStatus.Degraded,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            )
            .AddSqs(
                configuration,
                "Gateway customs declarations",
                sp => sp.GetRequiredService<IOptions<CustomsDeclarationsConsumerOptions>>().Value.QueueName,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            )
            .AddSqs(
                configuration,
                "Matched GMRs from BTMS",
                sp => sp.GetRequiredService<IOptions<MatchedGmrConsumerOptions>>().Value.QueueName,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            )
            .AddDataApi(
                sp => sp.GetRequiredService<IOptions<DataApiOptions>>().Value,
                tags: [WebApplicationExtensions.Extended],
                timeout: TimeSpan.FromSeconds(10)
            );

        return services;
    }
}
