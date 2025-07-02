using System.Net;
using System.Text.Json;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class AzureConsumerErrorHandler<T>(ILogger<AzureConsumerErrorHandler<T>> logger)
    : ServiceBusConsumerErrorHandler<T>
{
    public override async Task<ProcessResult> OnHandleError(
        T message,
        IConsumerContext consumerContext,
        Exception exception,
        int attempts
    )
    {
        switch (exception)
        {
            case HttpRequestException { StatusCode: HttpStatusCode.Conflict } when attempts < 3:
            {
                var delay = attempts * 250 + (Random.Shared.Next(1000) - 500);
                await Task.Delay(delay, consumerContext.CancellationToken);

                return Retry();
            }
            case JsonException jsonException:
                logger.LogWarning(jsonException, "Dead letter message early due to JSON deserialisation exception");

                return DeadLetter();
            default:
                return ProcessResult.Failure;
        }
    }
}
