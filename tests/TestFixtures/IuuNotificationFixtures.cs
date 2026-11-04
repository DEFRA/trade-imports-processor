using AutoFixture;
using AutoFixture.Dsl;
using Defra.TradeImportsProcessor.Processor.Models.Iuu;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class IuuNotificationFixtures
{
    private static Fixture GetFixture()
    {
        return new Fixture();
    }

    public static IPostprocessComposer<IuuNotification> IuuNotificationFixture(
        string reference = "IUU.FR.2026.0000001",
        DateTime? lastUpdated = null,
        string? iuuCheckOutcome = "IUUOK"
    )
    {
        return GetFixture()
            .Build<IuuNotification>()
            .With(iuuNotification => iuuNotification.Reference, reference)
            .With(
                iuuNotification => iuuNotification.LastUpdated,
                lastUpdated ?? new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc)
            )
            .With(iuuNotification => iuuNotification.IuuCheckOutcome, iuuCheckOutcome);
    }
}
