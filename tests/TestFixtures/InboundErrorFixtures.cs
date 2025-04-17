using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class InboundErrorFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<InboundError> InboundErrorFixture()
    {
        return GetFixture().Build<InboundError>();
    }
}
