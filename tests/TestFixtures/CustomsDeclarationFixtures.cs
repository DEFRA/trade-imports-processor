using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using BtmsGateway.Contract.CustomsDeclarations;

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
        var timestamp = serviceCallTimestamp ?? DateTime.UtcNow;

        return fixture
            .Build<ServiceHeader>()
            .With(sh => sh.DestinationSystem, "ALVS")
            .With(sh => sh.ServiceCallTimestamp, timestamp)
            .With(sh => sh.ServiceCallTimestampXml, timestamp.ToString("O"))
            .With(sh => sh.SourceSystem, "CDS")
            .With(sh => sh.CorrelationId, correlationId);
    }
}
