using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Metrics;
using NSubstitute;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class AzureConsumerErrorHandlerTests
{
    [Fact]
    public async Task WhenFirstAttempt_DoesNotIncrementMetric()
    {
        var consumerMetrics = Substitute.For<IConsumerMetrics>();
        var subject = new AzureConsumerErrorHandler<object>(consumerMetrics);

        var result = await subject.OnHandleError(new { }, new ConsumerContext(), new Exception(), 1);

        result.Should().Be(ProcessResult.Failure);
        consumerMetrics
            .Received(0)
            .DeadLetter(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Exception>());
    }

    [Fact]
    public async Task WhenFinalAttempt_IncrementsMetric()
    {
        var consumerMetrics = Substitute.For<IConsumerMetrics>();
        var subject = new AzureConsumerErrorHandler<object>(consumerMetrics);
        var exception = new Exception();

        var result = await subject.OnHandleError(
            new { },
            new ConsumerContext
            {
                Path = "path",
                Consumer = new FixtureConsumer(),
                Headers = new Dictionary<string, object>().AsReadOnly(),
            },
            exception,
            10
        );

        result.Should().Be(ProcessResult.Failure);
        consumerMetrics
            .Received(1)
            .DeadLetter(queueName: "path", consumerName: "FixtureConsumer", resourceType: "Unknown", exception);
    }

    [Fact]
    public async Task WhenJsonDeserializationError_DeadLetterEarly()
    {
        var consumerMetrics = Substitute.For<IConsumerMetrics>();
        var subject = new AzureConsumerErrorHandler<object>(consumerMetrics);
        var exception = new JsonException("deserialization", "path", lineNumber: 1, bytePositionInLine: 1);

        var result = await subject.OnHandleError(
            new { },
            new ConsumerContext
            {
                Path = "path",
                Consumer = new FixtureConsumer(),
                Headers = new Dictionary<string, object>().AsReadOnly(),
            },
            exception,
            1
        );

        result.Should().Be(ServiceBusProcessResult.DeadLetter);

        consumerMetrics
            .Received(1)
            .DeadLetter(queueName: "path", consumerName: "FixtureConsumer", resourceType: "Unknown", exception);
    }

#pragma warning disable S2094
    private class FixtureConsumer;
#pragma warning restore S2094
}
