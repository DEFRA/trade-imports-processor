using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Helpers;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using Xunit.Abstractions;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.InboundErrorFixtures;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesWireMockClient")]
public class CustomsDeclarationsConsumerTests(ITestOutputHelper output, WireMockClient wireMockClient)
    : SqsTestBase(output)
{
    private readonly IWireMockAdminApi _wireMockAdminApi = wireMockClient.WireMockAdminApi;

    private static Dictionary<string, MessageAttributeValue> WithInboundHmrcMessageType(string messageType)
    {
        return new Dictionary<string, MessageAttributeValue>
        {
            {
                "InboundHmrcMessageType",
                new MessageAttributeValue { DataType = "String", StringValue = messageType }
            },
        };
    }

    [Fact]
    public async Task WhenClearanceRequestSent_ThenClearanceRequestIsProcessedAndSentToTheDataApi()
    {
        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        var createPath = $"/customs-declarations/{mrn}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        await SendMessage(
            mrn,
            JsonSerializer.Serialize(clearanceRequest),
            WithInboundHmrcMessageType(InboundHmrcMessageType.ClearanceRequest)
        );

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var requestsModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                var requests = await _wireMockAdminApi.FindRequestsAsync(requestsModel);
                return requests.Count == 1;
            })
        );
    }

    [Fact]
    public async Task WhenFinalisationSent_ThenFinalisationIsProcessedAndSentToTheDataApi()
    {
        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn).Create();

        var createPath = $"/customs-declarations/{mrn}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        await SendMessage(
            mrn,
            JsonSerializer.Serialize(finalisation),
            WithInboundHmrcMessageType(InboundHmrcMessageType.Finalisation)
        );

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var requestsModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                var requests = await _wireMockAdminApi.FindRequestsAsync(requestsModel);
                return requests.Count == 1;
            })
        );
    }

    [Fact]
    public async Task WhenInboundErrorSent_TheInboundErrorIsProcessedAndSentToTheDataApi()
    {
        var mrn = GenerateMrn();
        var inboundError = InboundErrorFixture(mrn).Create();

        var createPath = $"/customs-declarations/{mrn}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(createPath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        await SendMessage(
            mrn,
            JsonSerializer.Serialize(inboundError),
            WithInboundHmrcMessageType(InboundHmrcMessageType.InboundError)
        );

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var requestsModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                var requests = await _wireMockAdminApi.FindRequestsAsync(requestsModel);
                return requests.Count == 1;
            })
        );
    }
}
