using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Services;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

public class NotificationConsumerTests : ServiceBusTestBase
{
    [Fact]
    public async Task WhenNotificationSent_ThenNotificationReceivedAndRemovedFromServiceBus()
    {
        await Sender.SendMessageAsync(new ServiceBusMessage { Body = new BinaryData("{ \"hello\": \"world\" }") });

        await Task.Delay(5000);

        var message = await Receiver.PeekMessageAsync();
        Assert.Null(message);
    }
}
