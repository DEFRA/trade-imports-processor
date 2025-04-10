using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using ImportNotificationStatus = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotificationStatus;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        return fixture;
    }

    public static IPostprocessComposer<ImportNotification> ImportNotificationFixture()
    {
        return GetFixture()
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, "CHEDP.GB.2025.1234567") // TO-DO: Randomize
            .With(i => i.Status, ImportNotificationStatus.InProgress);
    }

    public static IPostprocessComposer<ImportPreNotification> DataApiImportNotificationFixture()
    {
        var fixture = GetFixture();

        return fixture.Build<ImportPreNotification>().With(i => i.ReferenceNumber, "CHEDP.GB.2025.1234567"); // TO-DO: Randomize
    }
}
