using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.TradeImportsProcessor.Processor.Health;

[ExcludeFromCodeCoverage]
public class ServiceBusHealthCheck(string connectionString) : IHealthCheck
{
    private const string EmulatorHealthScheme = "http://";
    private const string EmulatorHealthPort = ":5300";

    /// <summary>
    /// The built in health checker AzureServiceBusTopicHealthCheck isn't currently compatible with the Azure Service Bus Emulator so this custom health check provides a mechanism for checking against the exposed /health endpoint of the emulator
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var emulatorHost = GetHostFromConnectionString(connectionString);

            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(emulatorHost);

            var response = await httpClient.GetAsync("/health", cancellationToken);

            response.EnsureSuccessStatusCode();

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                exception: new Exception("Failed to connect to Azure Service Bus Emulator", ex)
            );
        }
    }

    private static string GetHostFromConnectionString(string connectionString)
    {
        var start = connectionString.IndexOf("://", StringComparison.CurrentCultureIgnoreCase) + 3;
        var end = connectionString.IndexOf(";", StringComparison.CurrentCultureIgnoreCase) - start;

        return $"{EmulatorHealthScheme}{connectionString.Substring(start, end)}{EmulatorHealthPort}";
    }
}
