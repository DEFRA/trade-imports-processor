using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
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
        var subject = new AzureConsumerErrorHandler<object>(
            consumerMetrics,
            CreateOptions(),
            NullLogger<AzureConsumerErrorHandler<object>>.Instance
        );

        var result = await subject.OnHandleError(new { }, new ConsumerContext(), new Exception(), 1);

        result.Should().Be(ProcessResult.Failure);
        consumerMetrics
            .Received(0)
            .DeadLetter(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Exception>());
    }

    [Fact]
    public async Task WhenFinalAttempt_IncrementsMetric()
    {
        const int tolerance = 2;
        var consumerMetrics = Substitute.For<IConsumerMetrics>();
        var subject = new AzureConsumerErrorHandler<object>(
            consumerMetrics,
            CreateOptions(tolerance),
            NullLogger<AzureConsumerErrorHandler<object>>.Instance
        );
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
            tolerance
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
        var subject = new AzureConsumerErrorHandler<object>(
            consumerMetrics,
            CreateOptions(),
            NullLogger<AzureConsumerErrorHandler<object>>.Instance
        );
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

    private static OptionsWrapper<ServiceBusOptions> CreateOptions(int attemptsDeadLetterTolerance = 10) =>
        new(
            new ServiceBusOptions
            {
                Gmrs = new ServiceBusSubscriptionOptions
                {
                    ConnectionString = nameof(ServiceBusSubscriptionOptions.ConnectionString),
                    Topic = nameof(ServiceBusSubscriptionOptions.Topic),
                    Subscription = nameof(ServiceBusSubscriptionOptions.Subscription),
                },
                Notifications = new ServiceBusSubscriptionOptions
                {
                    ConnectionString = nameof(ServiceBusSubscriptionOptions.ConnectionString),
                    Topic = nameof(ServiceBusSubscriptionOptions.Topic),
                    Subscription = nameof(ServiceBusSubscriptionOptions.Subscription),
                },
                AttemptsDeadLetterTolerance = attemptsDeadLetterTolerance,
            }
        );

#pragma warning disable S2094
    private class FixtureConsumer;
#pragma warning restore S2094
}
