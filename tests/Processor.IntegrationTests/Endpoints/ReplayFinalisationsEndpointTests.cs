using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentAssertions;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Endpoints;

[Collection("UsesWireMockClient")]
public class ReplayFinalisationsEndpointTests(WireMockClient wireMockClient)
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
    public async Task WhenPostingAFinalisation_ItIsHandled()
    {
        var mrn = GenerateMrn();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();
        var finalisationHeader = FinalisationHeaderFixture((int)clearanceRequest.ExternalVersion!, mrn)
            .With(h => h.FinalState, ((int)FinalStateValues.Cleared).ToString())
            .Create();
        var finalisation = FinalisationFixture(mrn).With(f => f.Header, finalisationHeader).Create();
        var content = JsonSerializer.Serialize(finalisation);
        var body = new StringContent(content, Encoding.UTF8, "application/json");

        var getCustomsDeclarationResponse = new CustomsDeclarationResponse(
            mrn,
            clearanceRequest,
            null,
            null,
            null,
            DateTime.Now,
            DateTime.Now,
            "12345"
        );

        var createPath = $"/customs-declarations/{mrn}";
        var getMappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        getMappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingGet().WithPath(createPath))
                .WithResponse(rsp =>
                {
                    rsp.WithBody(JsonSerializer.Serialize(getCustomsDeclarationResponse));
                    rsp.WithStatusCode(HttpStatusCode.OK);
                })
        );
        var getMappingBuilderResult = await getMappingBuilder.BuildAndPostAsync();
        Assert.Null(getMappingBuilderResult.Error);

        var putMappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        putMappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var putMappingBuilderResult = await putMappingBuilder.BuildAndPostAsync();
        Assert.Null(putMappingBuilderResult.Error);

        const string traceHeader = "x-cdp-request-id";
        var traceId = Guid.NewGuid().ToString("N");
        var httpClient = CreateHttpClient();
        httpClient.DefaultRequestHeaders.Add(traceHeader, traceId);
        var response = await httpClient.PostAsync("/replay/finalisations", body);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

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
    public async Task WhenPostingAnInvalidBody_ItReturnsBadRequest()
    {
        var body = new StringContent("sosig", Encoding.UTF8, "application/json");
        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync("/replay/finalisations", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenPostingAnInvalidFinalisation_ItReturnsAnError()
    {
        var body = new StringContent("{}", Encoding.UTF8, "application/json");
        var httpClient = CreateHttpClient();
        var response = await httpClient.PostAsync("/replay/finalisations", body);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
