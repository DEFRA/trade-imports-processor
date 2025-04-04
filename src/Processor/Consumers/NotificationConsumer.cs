using System.Text.Json;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

internal class NotificationConsumer(ILogger<NotificationConsumer> logger) : IConsumer<Dictionary<string, object>>
{
    public Task OnHandle(Dictionary<string, object> message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification: {Message}", JsonSerializer.Serialize(message));
        return Task.CompletedTask;
    }
}
