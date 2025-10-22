using Defra.TradeImportsProcessor.Processor.Data.Entities;
using Defra.TradeImportsProcessor.Processor.Data.Extensions;

namespace Defra.TradeImportsProcessor.Processor.Tests.Extensions;

public class DataEntityExtensionsTests
{
    [Fact]
    public void NoAttributeTest()
    {
        typeof(NoAttributeClassEntity).DataEntityName().Should().Be("NoAttributeClass");
    }

    [Fact]
    public void AttributeTest()
    {
        typeof(AttributeClassEntity).DataEntityName().Should().Be("TestName");
    }

    public record NoAttributeClassEntity(string Test);

    [DbCollection("TestName")]
    public record AttributeClassEntity(string Test);
}
