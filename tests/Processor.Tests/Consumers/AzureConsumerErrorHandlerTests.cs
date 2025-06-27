using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging.Abstractions;
using SlimMessageBus.Host;
using SlimMessageBus.Host.AzureServiceBus;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class AzureConsumerErrorHandlerTests
{
    [Fact]
    public async Task WhenFirstAttempt_DoesNotIncrementMetric()
    {
        var subject = new AzureConsumerErrorHandler<object>(NullLogger<AzureConsumerErrorHandler<object>>.Instance);

        var result = await subject.OnHandleError(new { }, new ConsumerContext(), new Exception(), 1);

        result.Should().Be(ProcessResult.Failure);
    }

    [Fact]
    public async Task WhenFinalAttempt_IncrementsMetric()
    {
        const int tolerance = 2;
        var subject = new AzureConsumerErrorHandler<object>(NullLogger<AzureConsumerErrorHandler<object>>.Instance);
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
    }

    [Fact]
    public async Task WhenJsonDeserializationError_DeadLetterEarly()
    {
        var subject = new AzureConsumerErrorHandler<object>(NullLogger<AzureConsumerErrorHandler<object>>.Instance);
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
    }

#pragma warning disable S2094
    private class FixtureConsumer;
#pragma warning restore S2094
}
