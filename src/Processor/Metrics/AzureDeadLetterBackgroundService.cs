using System.Diagnostics.CodeAnalysis;
using Defra.TradeImportsProcessor.Processor.Configuration;

namespace Defra.TradeImportsProcessor.Processor.Metrics;

[ExcludeFromCodeCoverage]
public class AzureDeadLetterBackgroundService(
    ServiceBusSubscriptionOptions options,
    string consumerName,
    ILogger<AzureDeadLetterBackgroundService> logger,
    AzureMetrics azureMetrics,
    IDeadLetterService deadLetterService
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForRandomStartupDelay(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var count = await deadLetterService.PeekTotalMessageCount(stoppingToken);

                logger.LogInformation("Dead letter monitor for {Topic}, count {Count}", options.Topic, count);

                azureMetrics.DeadLetter(consumerName, count);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to get dead letter count for {Topic}", options.Topic);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    /// <summary>
    /// Wait for a random amount of time to minimise clashes as this will
    /// run in all deployed instances of the processor. We also try and
    /// mitigate hosts starting at the same time by using the machine name
    /// in the randomness calculation.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private static async Task WaitForRandomStartupDelay(CancellationToken cancellationToken)
    {
        var instanceId = Environment.MachineName.GetHashCode();
        var baseDelay = Random.Shared.Next(30, 120);
        var adjustedDelay = Math.Abs((baseDelay + instanceId) % 90) + 30; // Keep within 30-120 range
        var wait = TimeSpan.FromSeconds(adjustedDelay);

        await Task.Delay(wait, cancellationToken);
    }
}
