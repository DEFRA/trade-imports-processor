using Defra.TradeImportsProcessor.Processor.Utils.CorrelationId;

namespace Defra.TradeImportsProcessor.Processor.Tests.Utils;

public class CorrelationIdTests
{
    [Fact]
    public void CorrelationId_ShouldBeGenerated()
    {
        var generator = new CorrelationIdGenerator();

        var id = generator.Generate();

        id.Length.Should().Be(20);
    }
}
