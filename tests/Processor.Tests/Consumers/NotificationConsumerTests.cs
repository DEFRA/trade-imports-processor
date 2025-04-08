using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class NotificationConsumerTests
{
    [Fact]
    public void OnHandle_ReturnsTaskCompleted()
    {
        var logger = Substitute.For<ILogger<NotificationConsumer>>();
        var consumer = new NotificationConsumer(logger);

        var importNotification = ImportNotificationFixture();

        var result = consumer.OnHandle(importNotification, CancellationToken.None);
        Assert.Equal(Task.CompletedTask, result);
    }
}
