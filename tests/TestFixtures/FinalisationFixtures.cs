using AutoFixture;
using AutoFixture.Dsl;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class FinalisationFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.Finalisation> DataApiFinalisationFixture()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.Finalisation>();
    }
}
