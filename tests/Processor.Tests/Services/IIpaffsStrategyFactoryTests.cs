using Defra.TradeImportsProcessor.Processor.Services;
using NSubstitute;

namespace Defra.TradeImportsProcessor.Processor.Tests.Services;

public class IIpaffsStrategyFactoryTests
{
    [Fact]
    public void WhenTryingToGetStrategyThatExistsForSubResourceType_ThenShouldReturnTrueAndTheStrategy()
    {
        var strategy = Substitute.For<IIpaffsStrategy>();
        strategy.SupportedSubResourceType.Returns("Test");
        var ipaffsStrategyFactory = new IpaffsStrategyFactory([strategy]);

        var result = ipaffsStrategyFactory.TryGetIpaffsStrategy("Test", out var strategyImplementation);

        result.Should().BeTrue();
        strategyImplementation.Should().NotBeNull();
    }

    [Fact]
    public void WhenTryingToGetStrategyThatDoesNotExistForSubResourceType_ThenShouldReturnFalseAndNullStrategy()
    {
        var strategy = Substitute.For<IIpaffsStrategy>();
        strategy.SupportedSubResourceType.Returns("Foo");
        var ipaffsStrategyFactory = new IpaffsStrategyFactory([strategy]);

        var result = ipaffsStrategyFactory.TryGetIpaffsStrategy("Test", out var strategyImplementation);

        result.Should().BeFalse();
        strategyImplementation.Should().BeNull();
    }
}
