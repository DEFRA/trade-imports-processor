using System.Net;
using System.Net.Http.Json;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using Trade.Gateway.Api.Contract.Certificate;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Endpoints;

[Collection("UsesWireMockClient")]
public class DevTracesChedsEndpointTests(WireMockClient wireMockClient, ITestOutputHelper output) : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    [Fact]
    public async Task WhenTracesChedPostedToDevEndpoint_ThenNotificationProcessedAndSentToTheDataApi()
    {
        const string chedReference = "CHEDPP.GB.2026.2596331";
        const string createPath = $"/traces-cheds/{chedReference}";

        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        var ched = new DefraUNVTDCHEDProfile
        {
            ExchangedDocument = new ExchangedDocument
            {
                Identifier = chedReference,
                NotificationStatusCode = "VALIDATED",
                IncludedNote =
                [
                    new IncludedNote
                    {
                        Content = [DateTime.UtcNow.ToString("o")],
                        SubjectCode = new CodedValue { Value = "LAST_UPDATE_DATETIME" },
                    },
                ],
            },
            SpecifiedConsignment = new Consignment(),
        };

        var traceId = Guid.NewGuid().ToString("N");

        var httpClient = CreateHttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, Testing.Endpoints.Dev.PostTracesCheds())
        {
            Content = JsonContent.Create(ched),
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
