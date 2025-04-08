using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Services;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

public class NotificationConsumerTests : ServiceBusTestBase
{
    [Fact]
    public async Task WhenNotificationSent_ThenNotificationReceivedAndRemovedFromServiceBus()
    {
        var importNotification = ImportNotificationFixture();

        await Sender.SendMessageAsync(new ServiceBusMessage { Body = new BinaryData(importNotification) });

        await Task.Delay(5000);

        var message = await Receiver.PeekMessageAsync();
        Assert.Null(message);
    }
}
