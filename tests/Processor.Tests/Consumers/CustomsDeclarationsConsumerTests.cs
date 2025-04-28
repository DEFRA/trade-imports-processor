using System.Text.Json;
using Amazon.SQS.Model;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.Errors;
using Defra.TradeImportsDataApi.Domain.ProcessingErrors;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceDecisionFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.ClearanceRequestFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.CustomsDeclarationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.FinalisationFixtures;
using static Defra.TradeImportsProcessor.TestFixtures.InboundErrorFixtures;
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
    private readonly IValidator<ClearanceRequestValidatorInput> _clearanceRequestValidator = Substitute.For<
        IValidator<ClearanceRequestValidatorInput>
    >();
    private readonly IValidator<FinalisationValidatorInput> _finalisationValidator = Substitute.For<
        IValidator<FinalisationValidatorInput>
    >();
    private readonly IValidator<ServiceHeader> _serviceHeaderValidation = Substitute.For<IValidator<ServiceHeader>>();

    public CustomsDeclarationsConsumerTests()
    {
        _clearanceRequestValidator
            .ValidateAsync(Arg.Any<ClearanceRequestValidatorInput>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [] });
        _finalisationValidator
            .ValidateAsync(Arg.Any<FinalisationValidatorInput>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [] });
        _serviceHeaderValidation
            .ValidateAsync(Arg.Any<ServiceHeader>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [] });
    }

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

        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = unknownMessageTypeContext,
        };
        var clearanceRequest = ClearanceRequestFixture().Create();

        await Assert.ThrowsAsync<CustomsDeclarationMessageTypeException>(() =>
            consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken)
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

        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = unknownMessageTypeContext,
        };
        var clearanceRequest = ClearanceRequestFixture().Create();

        await Assert.ThrowsAsync<CustomsDeclarationMessageTypeException>(() =>
            consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken)
        );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Common")]
    public async Task OnHandle_WhenCustomsDeclarationsMessageReceived_ButFailsServiceHeaderValidation_ItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        var validationError = new ValidationFailure("DestinationSystem", "Error Message", "Space")
        {
            ErrorCode = "ALVSVAL999",
        };

        var existingProcessingNotifications = new ProcessingError
        {
            Notifications =
            [
                new ErrorNotification
                {
                    ExternalCorrelationId = "ANOTHER-CORRELATION-ID",
                    ExternalVersion = 1,
                    Errors = [new ErrorItem { Code = "PREVIOUSCODE", Message = "An error message" }],
                },
            ],
        };
        var existingProcessingErrorResponse = new ProcessingErrorResponse(
            mrn,
            existingProcessingNotifications,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _serviceHeaderValidation
            .ValidateAsync(Arg.Any<ServiceHeader>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [validationError] });

        _mockApi.GetProcessingError(mrn, _cancellationToken).Returns(existingProcessingErrorResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi.DidNotReceive().GetCustomsDeclaration(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _mockApi
            .DidNotReceive()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        await _mockApi
            .Received()
            .PutProcessingError(
                mrn,
                Arg.Is<ProcessingError>(e =>
                    e.Notifications != null
                    && e.Notifications.Length == 2
                    && e.Notifications[0].Errors[0].Code == "PREVIOUSCODE"
                    && e.Notifications[1].Errors[0].Code == validationError.ErrorCode
                ),
                Arg.Any<string>(),
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "ClearanceRequest")]
    public async Task OnHandle_WhenClearanceRequestReceived_ButFailsClearanceRequestValidation_ItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();

        var validationError = new ValidationFailure("SupplementaryUnits", "Error Message", "Space")
        {
            ErrorCode = "ALVSVAL999",
        };

        _clearanceRequestValidator
            .ValidateAsync(Arg.Any<ClearanceRequestValidatorInput>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [validationError] });

        await consumer.OnHandle(JsonSerializer.SerializeToElement(clearanceRequest), _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "ClearanceRequest")]
    public async Task OnHandle_WhenClearanceRequestReceived_AndNoCustomsDeclarationRecordExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
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
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.ClearanceRequest),
        };

        var mrn = GenerateMrn();
        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = ClearanceRequestFixture(mrn).Create();
        var finalisation = DataApiFinalisationFixture().Create();
        var existingClearanceRequest = DataApiClearanceRequestFixture().Create();
        var inboundError = DataApiInboundErrorFixture().Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            existingClearanceRequest,
            clearanceDecision,
            finalisation,
            inboundError,
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
                    d.InboundError == inboundError
                    && d.ClearanceDecision == clearanceDecision
                    && d.ClearanceRequest != null
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
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
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
    [Trait("CustomsDeclarations", "InboundError")]
    public async Task OnHandle_WhenInboundErrorReceived_AndNoOtherInboundErrorsExist_ItAddsANewOne()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.InboundError),
        };

        var mrn = GenerateMrn();
        var inboundErrorItem = new InboundErrorItem { errorCode = "CODE", errorMessage = "An error" };
        var inboundError = InboundErrorFixture(mrn).With(i => i.Errors, [inboundErrorItem]).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(inboundError), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(d =>
                    d.InboundError!.Notifications![0].ExternalCorrelationId == inboundError.ServiceHeader.CorrelationId
                    && d.InboundError!.Notifications![0].ExternalVersion == inboundError.Header.EntryVersionNumber
                    && d.InboundError!.Notifications![0].Errors![0].Code == inboundError.Errors[0].errorCode
                    && d.InboundError!.Notifications![0].Errors![0].Message == inboundError.Errors[0].errorMessage
                ),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "InboundError")]
    public async Task OnHandle_WhenInboundErrorReceived_AndOtherInboundErrorsExist_ItAddsItToTheExistingOnes()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.InboundError),
        };

        var mrn = GenerateMrn();
        var inboundErrorItem = new InboundErrorItem { errorCode = "CODE", errorMessage = "An error" };
        var inboundError = InboundErrorFixture(mrn).With(i => i.Errors, [inboundErrorItem]).Create();
        var existingInboundErrors = DataApiInboundErrorFixture().Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            null,
            null,
            null,
            existingInboundErrors,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(inboundError), _cancellationToken);

        await _mockApi
            .Received()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(d =>
                    d.InboundError!.Notifications!.Length == existingInboundErrors!.Notifications!.Length + 1
                ),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_ButNoClearanceRequestExistsInTheDataApi_ItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn).Create();

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(null as CustomsDeclarationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(finalisation), _cancellationToken);

        await _mockApi.Received().GetCustomsDeclaration(mrn, _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_ButFailsFinalisationValidation_ItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();
        var finalisation = FinalisationFixture(mrn).Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            clearanceRequest,
            null,
            null,
            null,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _mockApi.GetCustomsDeclaration(mrn, _cancellationToken).Returns(response);

        var validationError = new ValidationFailure("FinalState", "Error Message", "Space")
        {
            ErrorCode = "ALVSVAL999",
        };

        _finalisationValidator
            .ValidateAsync(Arg.Any<FinalisationValidatorInput>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [validationError] });

        await consumer.OnHandle(JsonSerializer.SerializeToElement(finalisation), _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutCustomsDeclaration(
                Arg.Any<string>(),
                Arg.Any<DataApiCustomsDeclaration.CustomsDeclaration>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_AndNoFinalisationRecordExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();
        var finalisation = FinalisationFixture(mrn).Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            clearanceRequest,
            null,
            null,
            null,
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
                Arg.Is<DataApiCustomsDeclaration.CustomsDeclaration>(cd => cd.Finalisation != null),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_AndAFinalisationRecordAlreadyExists_ThenItIsUpdated()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn).Create();
        var existingFinalisation = DataApiFinalisationFixture().Create();

        var clearanceDecision = DataApiClearanceDecisionFixture().Create();
        var clearanceRequest = DataApiClearanceRequestFixture().Create();
        var inboundError = DataApiInboundErrorFixture().Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            clearanceRequest,
            clearanceDecision,
            existingFinalisation,
            inboundError,
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
                    && cd.Finalisation!.ExternalVersion == finalisation.Header.EntryVersionNumber
                    && cd.InboundError == inboundError
                ),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Fact]
    [Trait("CustomsDeclarations", "Finalisation")]
    public async Task OnHandle_WhenFinalisationReceived_ButExistingFinalisationIsNewer_ThenItIsSkipped()
    {
        var consumer = new CustomsDeclarationsConsumer(
            _mockLogger,
            _mockApi,
            _clearanceRequestValidator,
            _finalisationValidator,
            _serviceHeaderValidation
        )
        {
            Context = GetConsumerContext(InboundHmrcMessageType.Finalisation),
        };

        var mrn = GenerateMrn();
        var finalisation = FinalisationFixture(mrn, 1)
            .With(f => f.ServiceHeader, ServiceHeaderFixture(DateTime.UtcNow.AddMinutes(-5)).Create())
            .Create();
        var existingFinalisation = DataApiFinalisationFixture().With(f => f.ExternalVersion, 2).Create();

        var response = new CustomsDeclarationResponse(
            mrn,
            null,
            null,
            existingFinalisation,
            null,
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
