using System.Security.Cryptography;
using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using ImportNotificationStatus = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotificationStatus;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    private static readonly List<string> s_chedTypes = ["A", "D", "P", "PP"];

    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        return fixture;
    }

    private static string GenerateReferenceNumber()
    {
        var chedType = s_chedTypes[RandomNumberGenerator.GetInt32(0, s_chedTypes.Count)];
        var currentYear = DateTime.Now.Year;
        var number = RandomNumberGenerator.GetInt32(1000000, 10000000);

        return $"CHED{chedType}.GB.{currentYear}.{number}";
    }

    public static IPostprocessComposer<ImportNotification> ImportNotificationFixture()
    {
        return GetFixture()
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, GenerateReferenceNumber())
            .With(i => i.LastUpdated, DateTime.UtcNow)
            .With(i => i.Status, ImportNotificationStatus.InProgress);
    }
}
