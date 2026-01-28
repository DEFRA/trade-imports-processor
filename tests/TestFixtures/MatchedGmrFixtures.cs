using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class MatchedGmrFixtures
{
    public static IPostprocessComposer<MatchedGmr> MatchedGmrFixture()
    {
        return new Fixture()
            .Build<MatchedGmr>()
            .With(x => x.Mrn, CustomsDeclarationFixtures.GenerateMrn())
            .With(x => x.Gmr, GmrFixtures.GmrFixture().Create());
    }
}
