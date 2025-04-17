using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
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
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();

    private readonly ILogger<CustomsDeclarationsConsumer> _mockLogger = Substitute.For<
        ILogger<CustomsDeclarationsConsumer>
    >();

    private static ConsumerContext GetConsumerContext(string inboundHmrcMessageType)
    {
        return new ConsumerContext
        {
            Headers = new Dictionary<string, object>
            {
                { "InboundHmrcMessageType", inboundHmrcMessageType },
            }.AsReadOnly(),
            Properties = { new KeyValuePair<string, object>("Sqs_Message", new Message { MessageId = "12345" }) },
        };
    }

    [Fact]
    [Trait("CustomsDeclarations", "Common")]
    public async Task OnHandle_WhenCustomsDeclarationsMessageReceived_ButItHasNoMessageType_AnExceptionIsThrown()
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
    [Trait("CustomsDeclarations", "Common")]
    public async Task OnHandle_WhenCustomsDeclarationsMessageReceived_ButItIsAnUnknownMessageType_AnExceptionIsThrown()
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
    [Trait("CustomsDeclarations", "ClearanceRequest")]
    public async Task OnHandle_WhenClearanceRequestReceived_AndNoCustomsDeclarationRecordExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(cd => cd.ClearanceRequest != null),
                null,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "ClearanceRequest")]
    public async Task OnHandle_WhenClearanceRequestReceived_AndACustomsDeclarationRecordAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var finalisation = DataApiFinalisationFixture().Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            clearanceDecision,
            finalisation,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
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
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "ClearanceRequest")]
    public async Task OnHandle_WhenClearanceRequestReceived_ButExistingClearanceRequestIsNewer_ThenItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn, 1).Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().With(cr => cr.ExternalVersion, 2).Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            null,
            null,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
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

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_AndNoCustomsDeclarationRecordExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(finalisation), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(cd => cd.Finalisation != null),
                null,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_AndACustomsDeclarationRecordAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn).Create();
        var existingFinalisation = DataApiFinalisationFixture().Create();

        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            clearanceRequest,
            clearanceDecision,
            existingFinalisation,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(finalisation), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                mrn,
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(cd =>
                    cd.ClearanceRequest == clearanceRequest
                    && cd.ClearanceDecision == clearanceDecision
                    && cd.Finalisation != null
                ),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_ButExistingFinalisationIsNewer_ThenItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(_mockLogger, _mockApi)
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn)
            .With(f => f.ServiceHeader, GenerateServiceHeader(DateTime.UtcNow.AddMinutes(-5)))
            .Create();
        var existingFinalisation = DataApiFinalisationFixture().With(f => f.MessageSentAt, DateTime.UtcNow).Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            null,
            null,
            existingFinalisation,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(finalisation), _cancellationToken);

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
