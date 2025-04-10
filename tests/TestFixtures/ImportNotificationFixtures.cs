using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    private static Fixture GetFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));
        return fixture;
    }

    public static ImportNotification ImportNotificationFixture()
    {
        return GetFixture()
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, "CHEDP.GB.2025.1234567") // TO-DO: Randomize
            .Create();
    }

    public static TradeImportsDataApi.Domain.Ipaffs.ImportPreNotification DataApiImportNotificationFixture()
    {
        var fixture = GetFixture();

        return fixture
            .Build<TradeImportsDataApi.Domain.Ipaffs.ImportPreNotification>()
            .With(i => i.ReferenceNumber, "CHEDP.GB.2025.1234567") // TO-DO: Randomize
            .Create();
    }
}
