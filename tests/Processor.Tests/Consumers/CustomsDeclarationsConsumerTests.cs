using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceDecisionFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
using Assert = Xunit.Assert;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class CustomsDeclarationsConsumerTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();

    private readonly ILogger<CustomsDeclarationsConsumer> _mockLogger = Substitute.For<
        ILogger<CustomsDeclarationsConsumer>
    >();

    private readonly ConsumerContext _testConsumerContext = new()
    {
        Headers = new Dictionary<string, object> { { "InboundHmrcMessageType", "ClearanceRequest" } }.AsReadOnly(),
        Properties = { new KeyValuePair<string, object>("Sqs_Message", new Message { MessageId = "12345" }) },
    };

    [Fact]
    public async Task OnHandle_WhenCustomsDeclarationMessageReceived_ButItHasNoMessageType_AnExceptionIsThrown()
    {
        var unknownMessageTypeContext = new ConsumerContext
        {
            Headers = new Dictionary<string, object>().AsReadOnly(),
            Properties = { new KeyValuePair<string, object>("Sqs_Message", new Message { MessageId = "12345" }) },
        };

        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi) { Context = unknownMessageTypeContext };
        var clearanceRequest = ClearanceRequestFixture().Create();

        await Assert.ThrowsAsync<CustomsDeclarationMessageTypeException>(
            () => consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken)
        );
    }

    [Fact]
    public async Task OnHandle_WhenCustomsDeclarationMessageReceived_ButItIsAnUnknownMessageType_AnExceptionIsThrown()
    {
        var unknownMessageTypeContext = new ConsumerContext
        {
            Headers = new Dictionary<string, object> { { "InboundHmrcMessageType", "Unknown" } }.AsReadOnly(),
            Properties = { new KeyValuePair<string, object>("Sqs_Message", new Message { MessageId = "12345" }) },
        };

        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi) { Context = unknownMessageTypeContext };
        var clearanceRequest = ClearanceRequestFixture().Create();

        await Assert.ThrowsAsync<CustomsDeclarationMessageTypeException>(
            () => consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken)
        );
    }

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_AndNoCustomsDeclarationRecordExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi) { Context = _testConsumerContext };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_AndACustomsDeclarationRecordAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi) { Context = _testConsumerContext };

        var mrn = GenerateMrn();
        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var finalisation = DataApiFinalisationFixture().Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        const string expectedEtag = "12345";

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            clearanceDecision,
            finalisation,
            DateTime.Now,
            DateTime.Now,
            expectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(d =>
                    d.ClearanceRequest != null
                    && d.ClearanceDecision == clearanceDecision
                    && d.Finalisation == finalisation
                ),
                expectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenClearanceRequestReceived_ButExistingClearanceRequestIsNewer_ThenItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi) { Context = _testConsumerContext };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn, 1).Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 2).Create();
        const string expectedEtag = "12345";

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            null,
            null,
            DateTime.Now,
            DateTime.Now,
            expectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
