using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class FinalisationFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    private static FinalisationHeader GenerateHeader(int version, string? mrn)
    {
        return GetFixture()
            .Build<FinalisationHeader>()
            .With(f => f.FinalState, "0")
            .With(f => f.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, version)
            .Create();
    }

    public static IPostprocessComposer<Finalisation> FinalisationFixture(string? mrn = null, int version = 2)
    {
        return GetFixture()
            .Build<Finalisation>()
            .With(f => f.Header, GenerateHeader(version, mrn))
            .With(f => f.ServiceHeader, ServiceHeaderFixture().Create());
    }

    public static IPostprocessComposer<DataApiCustomsDeclaration.Finalisation> DataApiFinalisationFixture(
        DateTime? messageSentAt = null
    )
    {
        return GetFixture()
            .Build<DataApiCustomsDeclaration.Finalisation>()
            .With(f => f.MessageSentAt, messageSentAt ?? DateTime.UtcNow.AddMinutes(-5))
            .With(f => f.ExternalVersion, 1);
    }
}
