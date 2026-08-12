using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using FluentAssertions;
using WireMock.Client;
using WireMock.Client.Extensions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Health;

[Collection("UsesWireMockClient")]
public class HealthTests(WireMockClient wireMockClient)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    [Fact]
    public async Task HealthAll_ShouldBeOk()
    {
        const string createPath = "/health/authorized";
        var getMappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        getMappingBuilder.Given(m =>
            m.WithRequest(req =>
                    req.UsingGet()
                        .WithPath(createPath)
                        .WithHeaders(x =>
                            x.Add(authHeader =>
                                authHeader
                                    .WithName("Authorization")
                                    .WithMatchers(authHeaderMatcher =>
                                        authHeaderMatcher.Add(authHeaderMatcherValue =>
                                            authHeaderMatcherValue
                                                .WithName("WildcardMatcher")
                                                .WithPattern(
                                                    $"Basic {Convert.ToBase64String("TradeImportsProcessor:secret"u8.ToArray())}"
                                                )
                                        )
                                    )
                            )
                        )
                )
                .WithResponse(rsp =>
                {
                    rsp.WithStatusCode(HttpStatusCode.OK);
                })
        );
        var getMappingBuilderResult = await getMappingBuilder.BuildAndPostAsync();
        Assert.Null(getMappingBuilderResult.Error);

        var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };

        var response = await client.GetAsync("/health/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<HealthResponse>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            ) ?? throw new InvalidOperationException("Could not deserialize response");

        // Azure service bus emulator does not support the management client. Therefore,
        // as the integration test uses the emulator, we expected the IPAFFS topic health check
        // to be degraded but all others should be healthy
        result.Status.Should().Be("Degraded");
        result.Results.Count.Should().Be(6);
        result.Results.Single(x => x.Key == "IPAFFS topic (outgoing)").Value.Status.Should().Be("Degraded");
        result
            .Results.Where(x => x.Key != "IPAFFS topic (outgoing)")
            .Should()
            .AllSatisfy(y => y.Value.Status.Should().Be("Healthy"));
    }

    private record HealthResponse(string Status, Dictionary<string, Result> Results);

    private record Result(string Status);
}
