using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ClearanceRequestFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    private static Header GenerateHeader(int version, string? mrn = null)
    {
        return GetFixture()
            .Build<Header>()
            .With(h => h.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, version)
            .Create();
    }

    public static IPostprocessComposer<ClearanceRequest> ClearanceRequestFixture(string? mrn = null, int version = 2)
    {
        return GetFixture().Build<ClearanceRequest>().With(c => c.Header, GenerateHeader(version, mrn));
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceRequest> DataApiClearanceRequestFixture()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.ClearanceRequest>().With(c => c.ExternalVersion, 1);
    }
}
