using System;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    public static ImportNotification ImportNotificationFixture()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(o => o.FromFactory((DateTime dt) => DateOnly.FromDateTime(dt)));

        return fixture
            .Build<ImportNotification>()
            .With(i => i.ReferenceNumber, "CHEDP.GB.2025.1234567") // TO-DO: Randomize
            .Create();
    }
}
