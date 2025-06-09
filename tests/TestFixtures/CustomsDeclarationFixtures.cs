using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class CustomsDeclarationFixtures
{
    private const string SValidMrnCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static string GenerateMrn()
    {
        var year = DateTime.Now.Year.ToString()[^2..];
        const string countryCode = "GB";
        var randomIdentifier = new string(
            Enumerable
                .Repeat(SValidMrnCharacters, 14)
                .Select(s => s[RandomNumberGenerator.GetInt32(0, SValidMrnCharacters.Length)])
                .ToArray()
        );

        return year + countryCode + randomIdentifier;
    }

    public static IPostprocessComposer<InboundErrorHeader> InboundErrorHeaderFixture(string? mrn = null)
    {
        return GetFixture()
            .Build<InboundErrorHeader>()
            .With(h => h.EntryReference, mrn ?? GenerateMrn())
            .With(h => h.EntryVersionNumber, 1);
    }

    public static IPostprocessComposer<ServiceHeader> ServiceHeaderFixture(DateTime? serviceCallTimestamp = null)
    {
        var fixture = GetFixture();
        var correlationId = fixture.Create<string>()[..20];

        return fixture
            .Build<ServiceHeader>()
            .With(sh => sh.DestinationSystem, "ALVS")
            .With(sh => sh.ServiceCallTimestamp, serviceCallTimestamp ?? DateTime.UtcNow)
            .With(sh => sh.SourceSystem, "CDS")
            .With(sh => sh.CorrelationId, correlationId);
    }
}
