using System.Security.Cryptography;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class CustomsDeclarationFixtures
{
    private const string SValidMrnCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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
}
