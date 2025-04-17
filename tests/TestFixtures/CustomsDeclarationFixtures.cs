using System.Security.Cryptography;
using AutoFixture;
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

    public static ServiceHeader GenerateServiceHeader(DateTime? serviceCallTimestamp = null)
    {
        return GetFixture()
            .Build<ServiceHeader>()
            .With(sh => sh.ServiceCallTimestamp, serviceCallTimestamp ?? DateTime.UtcNow)
            .Create();
    }
}
