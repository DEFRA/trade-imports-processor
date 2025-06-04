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
            .With(e => e.Header, InboundErrorHeaderFixture(mrn).Create())
            .With(e => e.ServiceHeader, ServiceHeaderFixture().Create())
            .With(e => e.Errors, [InboundErrorItemFixture().Create()]);
    }

    private static IPostprocessComposer<InboundErrorItem> InboundErrorItemFixture()
    {
        return GetFixture().Build<InboundErrorItem>().With(e => e.errorCode, "HMRCVAL101");
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.ExternalError> DataApiInboundErrorFixture()
    {
        return GetFixture().Build<DataApiCustomsDeclaration.ExternalError>();
    }
}
