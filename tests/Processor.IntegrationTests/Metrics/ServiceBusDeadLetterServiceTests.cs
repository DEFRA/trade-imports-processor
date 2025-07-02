using System.Net;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using Defra.TradeImportsProcessor.Processor.Metrics;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Metrics;

[Collection("UsesWireMockClient")]
public class ServiceBusDeadLetterServiceTests(WireMockClient wireMockClient, ITestOutputHelper testOutputHelper)
    : ServiceBusTestBase("notification-topic", "btms")
{
    [Fact]
    public async Task PeekTotalMessageCount_WhenMultipleBatchRequestsAreNeeded()
    {
        await using var subject = new ServiceBusDeadLetterService(
            2,
            new ServiceBusSubscriptionOptions
            {
                ConnectionString = ConnectionString,
                Topic = "notification-topic",
                Subscription = "btms",
                AutoStartConsumers = false,
            },
            new OptionsWrapper<CdpOptions>(new CdpOptions()),
            Substitute.For<IWebProxy>()
        );

        await ClearDeadLetter();

        const int expected = 9;

        for (var i = 0; i < expected; i++)
        {
            var message = new ServiceBusMessage { Body = new BinaryData("{ \"prop\": 1 }") };
            await Sender.SendMessageAsync(message);
        }

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var total = await subject.PeekTotalMessageCount(CancellationToken.None);

                    testOutputHelper.WriteLine($"Total {total}");

                    return total == expected;
                }
                catch (Exception)
                {
                    return false;
                }
            })
        );
    }
}
