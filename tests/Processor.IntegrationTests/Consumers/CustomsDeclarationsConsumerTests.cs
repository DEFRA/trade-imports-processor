using System.Net;
using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
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
using ClearanceRequest = Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations.ClearanceRequest;

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

    private async Task<Func<Task>> WithProcessingErrorEndpoint(string mrn)
    {
        var processingErrorCreatePath = $"/processing-errors/{mrn}";

        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(processingErrorCreatePath))
                .WithResponse(rsp => rsp.WithStatusCode(HttpStatusCode.Created))
        );
        var status = await mappingBuilder.BuildAndPostAsync();
        Assert.NotNull(status.Guid);

        return async () =>
            Assert.True(
                await AsyncWaiter.WaitForAsync(
                    async () =>
                    {
                        var requestsModel = new RequestModel { Methods = ["PUT"], Path = processingErrorCreatePath };
                        return (await _wireMockAdminApi.FindRequestsAsync(requestsModel)).Count == 0;
                    },
                    5
                )
            );
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

        var errorEndpointWasNotCalled = await WithProcessingErrorEndpoint(mrn);

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

        await errorEndpointWasNotCalled();
    }

    [Fact]
    public async Task WhenClearanceRequestSent_ButTheValidationFails_ThenAnErrorIsSentToTheDataApi()
    {
        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn)
            .With(c => c.ServiceHeader, ServiceHeaderFixture().With(s => s.DestinationSystem, "POTATO").Create())
            .Create();

        var processingErrorCreatePath = $"/processing-errors/{mrn}";
        var mappingBuilder = _wireMockAdminApi.GetMappingBuilder();
        mappingBuilder.Given(m =>
            m.WithRequest(req => req.UsingPut().WithPath(processingErrorCreatePath))
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
                var requestsModel = new RequestModel { Methods = ["PUT"], Path = processingErrorCreatePath };
                var requests = await _wireMockAdminApi.FindRequestsAsync(requestsModel);
                return requests.Count == 1;
            })
        );
    }

    [Fact]
    public async Task WhenClearanceRequestSent_ButNoItems_ThenClearanceRequestIsProcessedAndSentToTheDataApi()
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

        var errorEndpointWasNotCalled = await WithProcessingErrorEndpoint(mrn);

        var body = JsonSerializer.Serialize(
            clearanceRequest,
#pragma warning disable CA1869
            new JsonSerializerOptions
#pragma warning restore CA1869
            {
                Converters = { new ExcludePropertyJsonConverter<ClearanceRequest>(nameof(ClearanceRequest.Items)) },
            }
        );

        await SendMessage(mrn, body, WithInboundHmrcMessageType(InboundHmrcMessageType.ClearanceRequest));

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () =>
            {
                var requestsModel = new RequestModel { Methods = ["PUT"], Path = createPath };
                var requests = await _wireMockAdminApi.FindRequestsAsync(requestsModel);
                return requests.Count == 1;
            })
        );

        await errorEndpointWasNotCalled();
    }

    [Fact]
    public async Task WhenFinalisationSent_ThenFinalisationIsProcessedAndSentToTheDataApi()
    {
        var mrn = GenerateMrn();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();
        var finalisationHeader = FinalisationHeaderFixture((int)clearanceRequest.ExternalVersion!, mrn)
            .With(h => h.FinalState, ((int)FinalStateValues.Cleared).ToString())
            .Create();
        var finalisation = FinalisationFixture(mrn).With(f => f.Header, finalisationHeader).Create();

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

        var errorEndpointWasNotCalled = await WithProcessingErrorEndpoint(mrn);

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

        await errorEndpointWasNotCalled();
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

        var errorEndpointWasNotCalled = await WithProcessingErrorEndpoint(mrn);

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

        await errorEndpointWasNotCalled();
    }

    [Fact]
    public async Task WhenSendingInvalidMessage_ShouldEmitError()
    {
        var mrn = GenerateMrn();

        await DrainAllMessages();

        await SendMessage(
            mrn,
            """
            { "invalid": "json" }
            """,
            WithInboundHmrcMessageType(InboundHmrcMessageType.ClearanceRequest)
        );

        Assert.True(
            await AsyncWaiter.WaitForAsync(async () => (await GetQueueAttributes()).ApproximateNumberOfMessages == 0)
        );
    }
}
