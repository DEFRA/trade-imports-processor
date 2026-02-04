using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
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
using static Defra.TradeImportsProcessor.TestFixtures.MatchedGmrFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClient")]
public class MatchedGmrConsumerTests(ITestOutputHelper output, WireMockClient wireMockClient) : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;
    private const string MatchedGmrQueueUrl =
        "http://sqs.eu-west-2.127.0.0.1:4566/000000000000/trade_imports_matched_gmrs_btms_processor";

    [Fact]
    [Trait("Category", "IntegrationTest")]
    public async Task WhenMatchedGmrSent_ThenGmrProcessed_AndSentToTheDataApi()
    {
        var matchedGmr = MatchedGmrFixture().Create();
        var gmrId = matchedGmr.Gmr.GmrId!;

        var createPath = $"/gmrs/{gmrId}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        status.Guid.Should().NotBeNull();

        var messageGroupId = Guid.NewGuid().ToString("N");
        var traceId = Guid.NewGuid().ToString("N");

        var messageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            {
                MessageBusHeaders.TraceId,
                new MessageAttributeValue { DataType = "String", StringValue = traceId }
            },
            {
                MessageBusHeaders.ResourceId,
                new MessageAttributeValue { DataType = "String", StringValue = gmrId }
            },
        };

        await SendMessage(
            messageGroupId,
            JsonSerializer.Serialize(matchedGmr),
            MatchedGmrQueueUrl,
            messageAttributes,
            usesFifo: false
        );

        var assertionRequestModel = new RequestModel { Methods = ["PUT"], Path = createPath };

        (
            await AsyncWaiter.WaitForAsync(async () =>
                (await _wireMockAdminApi.FindRequestsAsync(assertionRequestModel)).Count == 1
            )
        )
            .Should()
            .BeTrue();
    }
}
