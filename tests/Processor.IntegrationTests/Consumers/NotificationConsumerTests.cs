using System.Net;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClient")]
public class NotificationConsumerTests(WireMockClient wireMockClient) : ServiceBusTestBase("notification-topic", "btms")
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    [Fact]
    public async Task WhenNotificationSent_ThenNotificationProcessedAndSentToTheDataApi()
    {
        var importNotification = ImportNotificationFixture().Create();

        var createPath = $"/import-pre-notifications/{importNotification.ReferenceNumber}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        var message = new ServiceBusMessage { Body = new BinaryData(importNotification) };
        var traceId = Guid.NewGuid().ToString("N");
        const string traceHeader = "x-cdp-request-id";
        message.ApplicationProperties.Add(traceHeader, traceId);
        await Sender.SendMessageAsync(message);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var requestModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                    var requests = (await _wireMockAdminApi.FindRequestsAsync(requestModel)).Where(x =>
                        x.Request.Headers != null
                        && x.Request.Headers.ContainsKey(traceHeader)
                        && x.Request.Headers.TryGetValue(traceHeader, out var list)
                        && list.Contains(traceId)
                    );

                    return requests.Count() == 1;
                }
                catch (Exception)
                {
                    return false;
                }
            })
        );
    }

    [Fact]
    public async Task WhenNotificationCannotDeserialize_ShouldDeadLetter()
    {
        var message = new ServiceBusMessage { Body = new BinaryData("{ \"prop\": 1 }") };
        var traceId = Guid.NewGuid().ToString("N");
        const string traceHeader = "x-cdp-request-id";
        message.ApplicationProperties.Add(traceHeader, traceId);
        await Sender.SendMessageAsync(message);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var messages = await DeadLetterReceiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

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
