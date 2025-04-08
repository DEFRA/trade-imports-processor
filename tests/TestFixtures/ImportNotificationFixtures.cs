using Bogus;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

namespace Defra.TradeImportsProcessor.TestFixtures;

public static class ImportNotificationFixtures
{
    public static ImportNotification ImportNotificationFixture()
    {
        var importNotification = new Faker<ImportNotification>();
        // ...
        return importNotification;
    }
}
