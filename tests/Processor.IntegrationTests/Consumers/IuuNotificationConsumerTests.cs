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
using static Defra.TradeImportsProcessor.TestFixtures.IuuNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClient")]
public class IuuNotificationConsumerTests(WireMockClient wireMockClient, ITestOutputHelper output) : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;
    private const string IuuNotificationQueueUrl =
        "http://sqs.eu-west-2.127.0.0.1:4566/000000000000/trade_imports_iuu_notifications_btms_processor";

    [Fact]
    [Trait("Category", "IntegrationTests")]
    public async Task WhenIuuNotificationSent_ThenItIsProcessed_AndSentToDataApi()
    {
        var iuuNotification = IuuNotificationFixture().Create();

        var createIuuNotificationPath = $"/iuu-notifications/{iuuNotification.Reference}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingGet().WithPath(createIuuNotificationPath))
                .WithResponse(resp => resp.WithStatusCode(HttpStatusCode.NotFound))
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
                new MessageAttributeValue { DataType = "String", StringValue = iuuNotification.Reference }
            },
        };

        var messageId = await SendMessage(
            messageGroupId,
            JsonSerializer.Serialize(iuuNotification),
            IuuNotificationQueueUrl,
            messageAttributes,
            usesFifo: false
        );

        var assertionRequestModel = new RequestModel { Methods = ["PUT"], Path = createIuuNotificationPath };

        (
            await AsyncWaiter.WaitForAsync(async () =>
                (await _wireMockAdminApi.FindRequestsAsync(assertionRequestModel)).Count == 1
            )
        )
            .Should()
            .BeTrue();

        var settings = new VerifySettings();
        settings.IgnoreMember("id");
        settings.IgnoreMember("etag");

        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync(Testing.Endpoints.RawMessages.Get(messageId));
        await VerifyJson(await response.Content.ReadAsStringAsync(), settings);
    }
}
