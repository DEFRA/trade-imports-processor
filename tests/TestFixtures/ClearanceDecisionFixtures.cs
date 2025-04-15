using AutoFixture;
using AutoFixture.Dsl;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ClearanceDecisionFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceDecision> DataApiClearanceDecisionFixture()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.ClearanceDecision>();
    }
}
