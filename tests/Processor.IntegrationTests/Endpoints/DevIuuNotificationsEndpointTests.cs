using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using Xunit.Abstractions;
using static Defra.TradeImportsProcessor.TestFixtures.IuuNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Endpoints;

[Collection("UsesWireMockClient")]
public class DevIuuNotificationsEndpointTests(WireMockClient wireMockClient, ITestOutputHelper output)
    : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    [Theory]
    [Trait("Category", "IntegrationTests")]
    [InlineData("IUUOK")]
    [InlineData(null)]
    public async Task WhenIuuNotificationPostedToDevEndpoint_ThenNotificationProcessedAndSentToTheDataApi(
        string? checkOutcome
    )
    {
        var iuuNotification = IuuNotificationFixture(iuuCheckOutcome: checkOutcome).Create();
        var createPath = $"/iuu-notifications/{iuuNotification.Reference}";

        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        var traceId = Guid.NewGuid().ToString("N");

        var httpClient = CreateHttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, Testing.Endpoints.Dev.PostIuuNotifications())
        {
            Content = JsonContent.Create(iuuNotification),
        };
        request.Headers.Add(MessageBusHeaders.TraceId, traceId);

        var response = await httpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var requestModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                    var r = await _wireMockAdminApi.FindRequestsAsync(requestModel);
                    var requests = r.Where(x =>
                            x.Request?.Headers != null
                            && x.Request.Headers.ContainsKey(MessageBusHeaders.TraceId)
                            && x.Request.Headers.TryGetValue(MessageBusHeaders.TraceId, out var list)
                            && list.Contains(traceId)
                        )
                        .ToArray();

                    // ensure that the check outcome is correct
                    return requests.Length == 1
                        && JsonElement.Parse(requests.Single().Request!.Body!).GetProperty("iuuOption").GetString()
                            == checkOutcome;
                }
                catch (Exception)
                {
                    return false;
                }
            })
        );
    }
}
