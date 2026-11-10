using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Models.Iuu;
using static Defra.TradeImportsProcessor.TestFixtures.IuuNotificationFixtures;
using DataApiIuuNotification = Defra.TradeImportsDataApi.Domain.Iuu;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class IuuNotificationTests
{
    [Fact]
    public async Task IuuNotification_DeserializesCorrectly()
    {
        const string iuuNotification = """
            {
                "reference": "IUU.FR.2026.0000001",
                "lastUpdated": "2026-10-19T09:00:00Z",
                "iuuCheckOutcome": "IUUOK"
            }
            """;

        var deserializedIuuNotification = JsonSerializer.Deserialize<IuuNotification>(iuuNotification);
        await Verify(deserializedIuuNotification).DontScrubDateTimes();
    }

    [Fact]
    public async Task IuuNotificationToDataApiIuuNotification_IsCorrect()
    {
        var iuuNotification = IuuNotificationFixture().Create();

        var dataApiIuuNotification = (DataApiIuuNotification.IuuNotification)iuuNotification;
        await Verify(dataApiIuuNotification).DontScrubDateTimes();
    }
}
