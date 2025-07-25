using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Endpoints;

[Collection("UsesWireMockClient")]
public class ReplayClearanceRequestsEndpointTests(WireMockClient wireMockClient)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    private static HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("http://localhost:8080");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            "TWVzc2FnZVJlcGxheTpyZXBsYXktbWVzc2FnZQ=="
        );
        return httpClient;
    }

    [Fact]
    public async Task WhenPostingAClearanceRequest_ItIsHandled()
    {
        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var content = JsonSerializer.Serialize(clearanceRequest);
        var body = new StringContent(content, Encoding.UTF8, "application/json");

        var createPath = $"/customs-declarations/{mrn}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        var traceId = Guid.NewGuid().ToString("N");
        var httpClient = CreateHttpClient();
        httpClient.DefaultRequestHeaders.Add(MessageBusHeaders.TraceId, traceId);
        var response = await httpClient.PostAsync("/replay/clearance-requests", body);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                try
                {
                    var requestModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                    var requests = (await _wireMockAdminApi.FindRequestsAsync(requestModel)).Where(x =>
                        x.Request.Headers != null
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

    [Fact]
    public async Task WhenPostingAnInvalidBody_ItReturnsBadRequest()
    {
        var body = new StringContent("sosig", Encoding.UTF8, "application/json");
        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync("/replay/clearance-requests", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenPostingAnInvalidClearanceRequest_ItReturnsAnError()
    {
        var body = new StringContent("{}", Encoding.UTF8, "application/json");
        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync("/replay/clearance-requests", body);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
