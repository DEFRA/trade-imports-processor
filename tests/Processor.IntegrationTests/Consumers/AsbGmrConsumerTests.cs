using System.Net;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClientAndServiceBus")]
public class AsbGmrConsumerTests(WireMockClient wireMockClient, ServiceBusFixture serviceBusFixture) : TestBase.TestBase
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;
    private readonly ServiceBusFixtureClient _serviceBusFixtureClient = serviceBusFixture.GetClient(
        "defra.trade.dmp.outputgmrs.dev.1001.topic",
        "defra.trade.dmp.btms-ingest.dev.1001.subscription"
    );

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

        var body = new BinaryData(gmr);
        var message = new ServiceBusMessage { Body = body, MessageId = Guid.NewGuid().ToString("N") };
        await _serviceBusFixtureClient.Sender.SendMessageAsync(message);

        var assertionRequestModel = new RequestModel { Methods = ["PUT"], Path = createPath };

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
                (await _wireMockAdminApi.FindRequestsAsync(assertionRequestModel)).Count == 1
            )
        );

        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync(Testing.Endpoints.RawMessages.Get(message.MessageId));
        response.EnsureSuccessStatusCode();

        response = await httpClient.GetAsync(Testing.Endpoints.RawMessages.GetJson(message.MessageId));
        body.ToString().Should().Be(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task WhenGmrCannotDeserialize_ShouldDeadLetter()
    {
        var message = new ServiceBusMessage { Body = new BinaryData("{ \"gmrId\": 1 }") };
        var traceId = Guid.NewGuid().ToString("N");
        message.ApplicationProperties.Add(MessageBusHeaders.TraceId, traceId);
        message.ApplicationProperties.Add(MessageBusHeaders.ResourceId, "123");
        await _serviceBusFixtureClient.Sender.SendMessageAsync(message);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var messages = await _serviceBusFixtureClient.DeadLetterReceiver.ReceiveMessagesAsync(
                        10,
                        TimeSpan.FromSeconds(5)
                    );

                    return messages.FirstOrDefault(x =>
                            x.ApplicationProperties.TryGetValue(MessageBusHeaders.TraceId, out var traceIdValue)
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
