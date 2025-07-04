using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Metrics;
using NSubstitute;

namespace Defra.TradeImportsProcessor.Processor.Tests.Metrics;

public class ServiceBusReceiverExtensionsTests
{
    [Fact]
    public async Task PeekTotalMessageCount_WhenBatches_ReturnsAsExpected()
    {
        var mockReceiver = Substitute.For<ServiceBusReceiver>();
        const int batch = 2;

        mockReceiver
            .PeekMessagesAsync(batch, null, CancellationToken.None)
            .Returns(
                new List<ServiceBusReceivedMessage>
                {
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 1),
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 2),
                }
            );

        mockReceiver
            .PeekMessagesAsync(batch, 3, CancellationToken.None)
            .Returns(
                new List<ServiceBusReceivedMessage>
                {
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 3),
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 4),
                }
            );

        mockReceiver
            .PeekMessagesAsync(batch, 5, CancellationToken.None)
            .Returns(
                new List<ServiceBusReceivedMessage>
                {
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 5),
                }
            );

        var messages = await mockReceiver.PeekTotalMessageCount(batch, cancellationToken: CancellationToken.None);

        messages.Should().Be(5);
    }

    [Fact]
    public async Task PeekTotalMessageCount_WhenNoBatches_ReturnsAsExpected()
    {
        var mockReceiver = Substitute.For<ServiceBusReceiver>();
        const int batch = 10;

        mockReceiver
            .PeekMessagesAsync(batch, null, CancellationToken.None)
            .Returns(
                new List<ServiceBusReceivedMessage>
                {
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 1),
                    ServiceBusModelFactory.ServiceBusReceivedMessage(sequenceNumber: 2),
                }
            );

        var messages = await mockReceiver.PeekTotalMessageCount(batch, cancellationToken: CancellationToken.None);

        messages.Should().Be(2);
    }
}
