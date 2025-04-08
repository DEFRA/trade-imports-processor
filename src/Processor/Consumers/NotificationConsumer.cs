using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using SlimMessageBus;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<NotificationConsumer> logger) : IConsumer<ImportNotification>
{
    public Task OnHandle(ImportNotification message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received notification: {Message}", JsonSerializer.Serialize(message));
        return Task.CompletedTask;
    }
}
