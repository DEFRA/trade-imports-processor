using System.Net;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

public class NotificationConsumerTests : ServiceBusTestBase
{
    private readonly IWireMockAdminApi wireMockAdminApi = RestClient.For<IWireMockAdminApi>("http://localhost:9090");

    [Fact]
    public async Task WhenNotificationSent_ThenNotificationReceivedAndRemovedFromServiceBus()
    {
        var importNotification = ImportNotificationFixture().Create();

        await wireMockAdminApi.ResetMappingsAsync();
        await wireMockAdminApi.ResetRequestsAsync();

        var createPath = $"/import-pre-notifications/{importNotification.ReferenceNumber}";
        var mappingBuilder = wireMockAdminApi.GetMappingBuilder();
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
                    var requests = (await wireMockAdminApi.FindRequestsAsync(requestModel)).Where(x =>
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
}
