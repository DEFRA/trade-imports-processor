using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlimMessageBus.Host;

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
}
