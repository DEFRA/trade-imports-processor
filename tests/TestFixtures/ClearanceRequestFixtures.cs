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

    private static Header GenerateHeader(string? mrn = null)
    {
        return GetFixture().Build<Header>().With(h => h.EntryReference, mrn ?? GenerateMrn()).Create();
    }

    public static IPostprocessComposer<ClearanceRequest> ClearanceRequestFixture(string? mrn = null)
    {
        return GetFixture().Build<ClearanceRequest>().With(c => c.Header, GenerateHeader(mrn));
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ClearanceRequest> DataApiClearanceRequestFixture(
        string? mrn = null
    )
    {
        return GetFixture().Build<DataApiCustomsDeclaration.ClearanceRequest>();
    }
}
