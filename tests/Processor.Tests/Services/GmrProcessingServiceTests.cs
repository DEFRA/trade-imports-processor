using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Tests.Services;

public class GmrProcessingServiceTests
{
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ILogger<GmrProcessingService> _mockLogger = Substitute.For<ILogger<GmrProcessingService>>();
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly IValidator<DataApiGvms.Gmr> _mockValidator = Substitute.For<IValidator<DataApiGvms.Gmr>>();

    public GmrProcessingServiceTests()
    {
        _mockValidator
            .ValidateAsync(Arg.Any<DataApiGvms.Gmr>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [] });
    }

    [Fact]
    public async Task ProcessGmr_WhenGmrFailsValidation_ItIsNotSentToTheDataApi()
    {
        var gmr = (DataApiGvms.Gmr)GmrFixture().Create();
        var service = new GmrProcessingService(_mockLogger, _mockApi, _mockValidator);

        _mockValidator
            .ValidateAsync(Arg.Any<DataApiGvms.Gmr>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [new ValidationFailure("Id", "No id provided")] });

        await service.ProcessGmr(gmr, _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutGmr(Arg.Any<string>(), Arg.Any<DataApiGvms.Gmr>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessGmr_WhenValidGmrIsReceived_AndItDoesNotExist_ItIsSentToTheDataApiWithNullETag()
    {
        var gmr = (DataApiGvms.Gmr)GmrFixture().Create();
        var service = new GmrProcessingService(_mockLogger, _mockApi, _mockValidator);

        await service.ProcessGmr(gmr, _cancellationToken);

        await _mockApi.Received().PutGmr(gmr.Id!, gmr, null, _cancellationToken);
    }

    [Fact]
    public async Task ProcessGmr_WhenValidGmrIsReceived_AndItAlreadyExists_ThenItIsUpdatedWithETag()
    {
        var gmr = (DataApiGvms.Gmr)GmrFixture().Create();
        var existingGmr = (DataApiGvms.Gmr)
            GmrFixture()
                .With(x => x.UpdatedDateTime, DateTime.UtcNow.AddMinutes(-5))
                .With(x => x.GmrId, gmr.Id)
                .Create();
        var response = new GmrResponse(existingGmr, DateTime.Now, DateTime.Now, ExpectedEtag);

        _mockApi.GetGmr(gmr.Id!, _cancellationToken).Returns(response);

        var service = new GmrProcessingService(_mockLogger, _mockApi, _mockValidator);

        await service.ProcessGmr(gmr, _cancellationToken);

        await _mockApi.Received().PutGmr(gmr.Id!, gmr, ExpectedEtag, _cancellationToken);
    }

    [Fact]
    public async Task ProcessGmr_WhenValidGmrIsReceived_AndItAlreadyExists_AndItIsOlder_ThenItIsIgnored()
    {
        var gmr = (DataApiGvms.Gmr)GmrFixture().With(x => x.UpdatedDateTime, DateTime.UtcNow.AddMinutes(-5)).Create();
        var existingGmr = (DataApiGvms.Gmr)GmrFixture().With(x => x.GmrId, gmr.Id).Create();
        var response = new GmrResponse(existingGmr, DateTime.Now, DateTime.Now, ExpectedEtag);

        _mockApi.GetGmr(gmr.Id!, _cancellationToken).Returns(response);

        var service = new GmrProcessingService(_mockLogger, _mockApi, _mockValidator);

        await service.ProcessGmr(gmr, _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutGmr(Arg.Any<string>(), Arg.Any<DataApiGvms.Gmr>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
