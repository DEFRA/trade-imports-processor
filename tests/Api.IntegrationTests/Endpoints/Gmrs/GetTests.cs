using Defra.TradeImportsProcessor.Api.Services;
using Defra.TradeImportsProcessor.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WireMock.Server;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Api.IntegrationTests.Endpoints.Gmrs;

public class GetTests : EndpointTestBase, IClassFixture<WireMockContext>
{
    private IGmrService MockGmrService { get; } = Substitute.For<IGmrService>();

    private WireMockServer WireMock { get; }
    private HttpClient HttpClient { get; }
    private const string GmrId = "gmrId";
    private readonly VerifySettings _settings;

    public GetTests(ApiWebApplicationFactory factory, ITestOutputHelper outputHelper, WireMockContext context)
        : base(factory, outputHelper)
    {
        WireMock = context.Server;
        WireMock.Reset();
        HttpClient = context.HttpClient;

        _settings = new VerifySettings();
        _settings.ScrubMember("traceId");
    }

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);

        services.AddTransient<IGmrService>(_ => MockGmrService);
    }

    [Fact]
    public async Task Get_WhenNotFound_ShouldNotBeFound()
    {
        var client = CreateClient();

        var response = await client.GetAsync(TradeImportsProcessor.Testing.Endpoints.Gmrs.Get(GmrId));

        await VerifyJson(await response.Content.ReadAsStringAsync(), _settings);
    }

    [Fact]
    public async Task Get_WhenException_ShouldBeInternalServerError()
    {
        var client = CreateClient();
        MockGmrService.GetGmr(GmrId).Throws(new Exception("BOOM!"));

        var response = await client.GetAsync(TradeImportsProcessor.Testing.Endpoints.Gmrs.Get(GmrId));

        await VerifyJson(await response.Content.ReadAsStringAsync(), _settings);
    }

    [Fact]
    public async Task Get_WhenBadRequest_ShouldBeBadRequest()
    {
        var client = CreateClient();
        MockGmrService.GetGmr(GmrId).Throws(new BadHttpRequestException("Bad Request Detail"));

        var response = await client.GetAsync(TradeImportsProcessor.Testing.Endpoints.Gmrs.Get(GmrId));

        await VerifyJson(await response.Content.ReadAsStringAsync(), _settings);
    }
}
