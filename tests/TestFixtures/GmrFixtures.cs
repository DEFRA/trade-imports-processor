using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class GmrFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<Gmr> GmrFixture()
    {
        return GetFixture().Build<Gmr>().With(x => x.UpdatedDateTime, DateTime.UtcNow);
    }
}
