using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Data;
using Defra.TradeImportsProcessor.Processor.Data.Entities;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class RawMessageLoggingInterceptorTests
{
    [Fact]
    public async Task OnHandle_MessageIsString_CallsNextAndDoesNotInsert()
    {
        // Arrange
        var dbContext = Substitute.For<IDbContext>();
        var options = Options.Create(new RawMessageLoggingOptions { TtlDays = 7, Enabled = true });
        var logger = Substitute.For<ILogger<RawMessageLoggingInterceptor<string>>>();

        var interceptor = new RawMessageLoggingInterceptor<string>(dbContext, logger, options);

        var nextCalled = 0;
        var next = () =>
        {
            nextCalled++;
            return Task.FromResult<object>("next-result");
        };

        var context = new ConsumerContext
        {
            // minimal context: no headers so resource type will be inferred as Unknown
            CancellationToken = CancellationToken.None,
        };

        // Act
        var result = await interceptor.OnHandle("raw-string-message", next, context);

        // Assert
        result.Should().Be("next-result");
        nextCalled.Should().Be(1);
        dbContext.ReceivedCalls().Should().BeEmpty(); // no DB operations performed
    }

    [Fact]
    public async Task OnHandle_MatchedGmrMessage_LogsRawMessageWithGmrId()
    {
        // Arrange
        var dbContext = Substitute.For<IDbContext>();
        var rawMessages = Substitute.For<IMongoCollectionSet<RawMessageEntity>>();
        dbContext.RawMessages.Returns(rawMessages);
        var options = Options.Create(new RawMessageLoggingOptions { TtlDays = 7, Enabled = true });
        var logger = Substitute.For<ILogger<RawMessageLoggingInterceptor<MatchedGmr>>>();

        var interceptor = new RawMessageLoggingInterceptor<MatchedGmr>(dbContext, logger, options);

        var gmr = GmrFixture().With(x => x.GmrId, "GMR-123").Create();
        var matchedGmr = new MatchedGmr { Mrn = "23GB123456789012345", Gmr = gmr };

        var context = new ConsumerContext
        {
            Headers = new Dictionary<string, object>(),
            Consumer = new MatchedGmrConsumer(
                Substitute.For<ILogger<MatchedGmrConsumer>>(),
                Substitute.For<Defra.TradeImportsProcessor.Processor.Services.IGmrProcessingService>()
            )
            {
                Context = new ConsumerContext(),
            },
            CancellationToken = CancellationToken.None,
        };

        var next = () => Task.FromResult<object>("next-result");

        // Act
        var result = await interceptor.OnHandle(matchedGmr, next, context);

        // Assert
        result.Should().Be("next-result");
        rawMessages
            .Received(1)
            .Insert(
                Arg.Is<RawMessageEntity>(entity =>
                    entity.ResourceType == ResourceTypes.Gmr && entity.ResourceId == "GMR-123"
                )
            );
    }
}
