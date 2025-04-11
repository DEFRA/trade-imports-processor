using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using ImportNotificationStatus = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotificationStatus;

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
        var chedType = s_chedTypes[new Random().Next(s_chedTypes.Count)];
        var currentYear = DateTime.Now.Year;
        var number = new Random().Next(1000000, 10000000);

        return $"CHED{chedType}.GB.{currentYear}.{number}";
    }

    public static IPostprocessComposer<ImportNotification> ImportNotificationFixture()
    {
        return GetFixture()
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, GenerateReferenceNumber()) // TO-DO: Randomize
            .With(i => i.Status, ImportNotificationStatus.InProgress);
    }

    public static IPostprocessComposer<ImportPreNotification> DataApiImportNotificationFixture()
    {
        var fixture = GetFixture();

        return fixture.Build<ImportPreNotification>().With(i => i.ReferenceNumber, GenerateReferenceNumber()); // TO-DO: Randomize
    }
}
