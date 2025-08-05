using System.Net.Http.Headers;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;

public abstract class TestBase
{
    protected static HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            // See compose.yml for username, password and scope configuration
            Convert.ToBase64String("IntegrationTests:integration-tests-pwd"u8.ToArray())
        );

        return httpClient;
    }
}