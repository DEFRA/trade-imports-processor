using System.Text.Json;
using AutoFixture;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus.Host;
using static Defra.TradeImportsProcessor.TestFixtures.GmrFixtures;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class GmrConsumerTests
{
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ILogger<GmrsConsumer> _mockLogger = Substitute.For<ILogger<GmrsConsumer>>();
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly IValidator<DataApiGvms.Gmr> _mockValidator = Substitute.For<IValidator<DataApiGvms.Gmr>>();

    private static ConsumerContext GetConsumerContext()
    {
        return new ConsumerContext
        {
            Properties =
            {
                new KeyValuePair<string, object>(
                    "ServiceBus_Message",
                    ServiceBusModelFactory.ServiceBusReceivedMessage()
                ),
            },
        };
    }

    public GmrConsumerTests()
    {
        _mockValidator
            .ValidateAsync(Arg.Any<DataApiGvms.Gmr>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [] });
    }

    [Fact]
    public async Task OnHandle_WhenAGmrFailsValidation_ItIsNotSentToTheDataApi()
    {
        var gmr = GmrFixture().Create();
        var consumer = new GmrsConsumer(_mockLogger, _mockApi, _mockValidator) { Context = GetConsumerContext() };

        _mockValidator
            .ValidateAsync(Arg.Any<DataApiGvms.Gmr>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult { Errors = [new ValidationFailure("Id", "No id provided")] });

        await consumer.OnHandle(JsonSerializer.SerializeToElement(gmr), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutGmr(Arg.Any<string>(), Arg.Any<DataApiGvms.Gmr>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task OnHandle_WhenAValidGmrIsReceived_AndItDoesNotAlreadyExist_ItIsSentToTheDataApi()
    {
        var gmr = GmrFixture().Create();

        var consumer = new GmrsConsumer(_mockLogger, _mockApi, _mockValidator) { Context = GetConsumerContext() };

        await consumer.OnHandle(JsonSerializer.SerializeToElement(gmr), _cancellationToken);

        await _mockApi.Received().PutGmr(gmr.GmrId!, Arg.Any<DataApiGvms.Gmr>(), null, _cancellationToken);
    }

    [Fact]
    public async Task OnHandle_WhenAValidGmrIsReceived_AndItAlreadyExists_ThenItIsUpdated()
    {
        var gmr = GmrFixture().Create();
        var dataApiGmr = (DataApiGvms.Gmr)
            GmrFixture()
                .With(x => x.UpdatedDateTime, DateTime.UtcNow.AddMinutes(-5))
                .With(x => x.GmrId, gmr.GmrId)
                .Create();
        var response = new GmrResponse(dataApiGmr, DateTime.Now, DateTime.Now, ExpectedEtag);

        _mockApi.GetGmr(gmr.GmrId!, _cancellationToken).Returns(response);

        var consumer = new GmrsConsumer(_mockLogger, _mockApi, _mockValidator) { Context = GetConsumerContext() };

        await consumer.OnHandle(JsonSerializer.SerializeToElement(gmr), _cancellationToken);

        await _mockApi.Received().PutGmr(gmr.GmrId!, Arg.Any<DataApiGvms.Gmr>(), ExpectedEtag, _cancellationToken);
    }

    [Fact]
    public async Task OnHandle_WhenAValidGmrIsReceived_AndItAlreadyExists_AndItIsOlder_ThenItIsIgnored()
    {
        var gmr = GmrFixture().With(x => x.UpdatedDateTime, DateTime.UtcNow.AddMinutes(-5)).Create();
        var dataApiGmr = (DataApiGvms.Gmr)GmrFixture().With(x => x.GmrId, gmr.GmrId).Create();
        var response = new GmrResponse(dataApiGmr, DateTime.Now, DateTime.Now, ExpectedEtag);

        _mockApi.GetGmr(gmr.GmrId!, _cancellationToken).Returns(response);

        var consumer = new GmrsConsumer(_mockLogger, _mockApi, _mockValidator) { Context = GetConsumerContext() };

        await consumer.OnHandle(JsonSerializer.SerializeToElement(gmr), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutGmr(Arg.Any<string>(), Arg.Any<DataApiGvms.Gmr>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
