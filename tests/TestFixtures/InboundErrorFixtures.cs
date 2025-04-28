using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class InboundErrorFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<InboundError> InboundErrorFixture(string? mrn = null)
    {
        return GetFixture()
            .Build<InboundError>()
            .With(e => e.Header, HeaderFixture(mrn).Create())
            .With(e => e.ServiceHeader, ServiceHeaderFixture().Create());
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.InboundError> DataApiInboundErrorFixture()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.InboundError>();
    }
}
