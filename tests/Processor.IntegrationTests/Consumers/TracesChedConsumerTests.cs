using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using Trade.Gateway.Api.Contract.Certificate;
using Trade.Gateway.Api.Contract.Events;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using Xunit.Abstractions;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClientAndServiceBus")]
public class TracesChedConsumerTests(WireMockClient wireMockClient, ITestOutputHelper output) : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;
    private const string QueueUrl =
        "http://sqs.eu-west-2.127.0.0.1:4566/000000000000/trade_gateway_publisher_ched_updates_processor.fifo";

    [Fact]
    public async Task WhenTracesChdSent_ThenNotificationProcessedAndSentToTheDataApi()
    {
        var ched = new DefraUNVTDCHEDProfile
        {
            ExchangedDocument = new ExchangedDocument()
            {
                Identifier = "CHEDPP.GB.2026.2596331",
                NotificationStatusCode = "VALIDATED",
                IncludedNote =
                [
                    new IncludedNote()
                    {
                        Content = [DateTime.UtcNow.ToString("o")],
                        SubjectCode = new CodedValue() { Value = "LAST_UPDATE_DATETIME" },
                    },
                ],
            },
            SpecifiedConsignment = new Consignment(),
            LastUpdated = DateTimeOffset.UtcNow,
        };

        var createPath = $"/traces-cheds/CHEDPP.GB.2026.2596331";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

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
                new MessageAttributeValue { DataType = "String", StringValue = "CHEDPP.GB.2026.2596331" }
            },
        };

        var messageId = await SendMessage(
            messageGroupId,
            JsonSerializer.Serialize(ched.ToEventEnvelope("testCorrelationId")),
            QueueUrl,
            messageAttributes,
            usesFifo: true
        );

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

        var settings = new VerifySettings();
        settings.IgnoreMember("id");
        settings.IgnoreMember("etag");
        settings.IgnoreMember("message");

        var httpClient = CreateHttpClient();
        var response = await httpClient.GetAsync(Testing.Endpoints.RawMessages.Get(messageId));
        await VerifyJson(await response.Content.ReadAsStringAsync(), settings);
    }
}
