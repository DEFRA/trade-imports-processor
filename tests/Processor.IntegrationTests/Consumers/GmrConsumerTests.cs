using System.Net;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClient")]
public class GmrConsumerTests(WireMockClient wireMockClient)
    : ServiceBusTestBase(
        "defra.trade.dmp.outputgmrs.dev.1001.topic",
        "defra.trade.dmp.btms-ingest.dev.1001.subscription"
    )
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    [Fact]
    public async Task WhenGmrSent_ThenGmrProcessed_AndSentToTheDataApi()
    {
        var gmr = GmrFixture().Create();

        var createPath = $"/gmrs/{gmr.GmrId}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        var message = new ServiceBusMessage { Body = new BinaryData(gmr) };
        await Sender.SendMessageAsync(message);

        var assertionRequestModel = new RequestModel { Methods = ["PUT"], Path = createPath };

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
                (await _wireMockAdminApi.FindRequestsAsync(assertionRequestModel)).Count == 1
            )
        );
    }

    [Fact]
    public async Task WhenGmrCannotDeserialize_ShouldDeadLetter()
    {
        var message = new ServiceBusMessage { Body = new BinaryData("{ \"GmrId\": 1 }") };
        var traceId = Guid.NewGuid().ToString("N");
        const string traceHeader = "x-cdp-request-id";
        message.ApplicationProperties.Add(traceHeader, traceId);
        await Sender.SendMessageAsync(message);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var messages = await DeadLetterReceiver.ReceiveMessagesAsync(10);

                    return messages.FirstOrDefault(x =>
                            x.ApplicationProperties.TryGetValue(traceHeader, out var traceIdValue)
                            && traceIdValue.ToString() == traceId
                        ) != null;
                }
                catch (Exception)
                {
                    return false;
                }
            })
        );
    }
}
