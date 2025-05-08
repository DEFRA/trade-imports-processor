using System.Net;
using FluentAssertions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Health;

public class HealthTests
{
    [Fact]
    public async Task HealthAll_ShouldBeOk()
    {
        var client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };

        var response = await client.GetAsync("/health/all");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
