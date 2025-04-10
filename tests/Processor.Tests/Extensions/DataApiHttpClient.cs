using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Defra.TradeImportsProcessor.Processor.Tests.Clients;

public class DataApiHttpClient
{
    [Theory]
    [InlineData("http://localhost", 1, 1)]
    [InlineData("https://localhost", 2, 0)]
    public void AddDataApiHttpClient_ConfiguresCorrectly(string baseAddress, int expectedMajor, int expectedMinor)
    {
        var dataApiOptions = new DataApiOptions
        {
            BaseAddress = baseAddress,
            Username = "username",
            Password = "password",
        };
        var expectedBasicAuthCredential = "dXNlcm5hbWU6cGFzc3dvcmQ=";

        var services = new ServiceCollection();
        services.AddSingleton(Options.Create(dataApiOptions));
        services.AddDataApiHttpClient();
        var sp = services.BuildServiceProvider();

        var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
        var dataApiHttpClient = httpClientFactory.CreateClient(nameof(ITradeImportsDataApiClient));

        dataApiHttpClient.Should().NotBeNull();
        dataApiHttpClient.BaseAddress.Should().Be(new Uri(dataApiOptions.BaseAddress));
        dataApiHttpClient.DefaultRequestVersion.Should().Be(new Version(expectedMajor, expectedMinor));
        dataApiHttpClient.DefaultRequestHeaders.Authorization?.Scheme.Should().Be("Basic");
        dataApiHttpClient.DefaultRequestHeaders.Authorization?.Parameter.Should().Be(expectedBasicAuthCredential);
    }
}
