using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class NotificationConsumerTests
{
    [Fact]
    public void OnHandle_ReturnsTaskCompleted()
    {
        var logger = Substitute.For<ILogger<NotificationConsumer>>();
        var consumer = new NotificationConsumer(logger);

        var result = consumer.OnHandle(new Dictionary<string, object>(), CancellationToken.None);
        Assert.Equal(Task.CompletedTask, result);
    }
}
