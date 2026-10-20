using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.Iuu;
using Defra.TradeImportsProcessor.Processor.Validation.Iuu;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.IuuNotificationFixtures;
using DataApiIuu = Defra.TradeImportsDataApi.Domain.Iuu;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class IuuNotificationConsumerTests
{
    private readonly ILogger<IuuNotificationConsumer> _mockLogger = Substitute.For<ILogger<IuuNotificationConsumer>>();
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly IValidator<IuuNotification> _validator = new IuuNotificationValidator();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IuuNotificationConsumer _subject;

    public IuuNotificationConsumerTests()
    {
        _subject = new IuuNotificationConsumer(_mockLogger, _mockApi, _validator);
    }

    [Fact]
    public async Task OnHandle_WhenIuuNotificationIsInvalid_ThrowsException()
    {
        var iuuNotification = IuuNotificationFixture(iuuCheckOutcome: "INVALID_VALUE").Create();

        await Assert.ThrowsAsync<ArgumentException>(() => _subject.OnHandle(iuuNotification, _cancellationToken));
    }

    [Fact]
    public async Task OnHandle_WhenExistingIuuNotificationDoesNotExist_ThenIuuNotificationIsCreated()
    {
        var iuuNotification = IuuNotificationFixture().Create();

        _mockApi
            .GetIuuNotification(iuuNotification.Reference, _cancellationToken)
            .Returns(null as IuuNotificationResponse);

        await _subject.OnHandle(iuuNotification, _cancellationToken);

        await _mockApi
            .Received()
            .PutIuuNotification(
                iuuNotification.Reference,
                Arg.Any<DataApiIuu.IuuNotification>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenNewerIuuNotificationReceived_ThenIuuNotificationIsUpdated()
    {
        var iuuReference = "IUU.FR.2026.0000001";

        var existingIuuNotification = new DataApiIuu.IuuNotification
        {
            Reference = iuuReference,
            LastUpdated = new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
            IuuOption = "IUUOK",
        };

        _mockApi
            .GetIuuNotification(iuuReference, _cancellationToken)
            .Returns(
                new IuuNotificationResponse(
                    existingIuuNotification,
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    "etag"
                )
            );

        var iuuNotification = IuuNotificationFixture(lastUpdated: new DateTime(2026, 10, 19, 9, 1, 0, DateTimeKind.Utc))
            .Create();

        await _subject.OnHandle(iuuNotification, _cancellationToken);

        await _mockApi
            .Received()
            .PutIuuNotification(iuuReference, Arg.Any<DataApiIuu.IuuNotification>(), "etag", _cancellationToken);
    }

    [Fact]
    public async Task OnHandle_WhenOlderIuuNotificationReceived_ThenIuuNotificationIsNotCreatedOrUpdated()
    {
        var iuuReference = "IUU.FR.2026.0000001";

        var existingIuuNotification = new DataApiIuu.IuuNotification
        {
            Reference = iuuReference,
            LastUpdated = new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
            IuuOption = "IUUOK",
        };

        _mockApi
            .GetIuuNotification(iuuReference, _cancellationToken)
            .Returns(
                new IuuNotificationResponse(
                    existingIuuNotification,
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    "etag"
                )
            );

        var iuuNotification = IuuNotificationFixture(
                lastUpdated: new DateTime(2026, 10, 19, 8, 59, 0, DateTimeKind.Utc)
            )
            .Create();

        await _subject.OnHandle(iuuNotification, _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutIuuNotification(iuuReference, Arg.Any<DataApiIuu.IuuNotification>(), null, _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutIuuNotification(iuuReference, Arg.Any<DataApiIuu.IuuNotification>(), "etag", _cancellationToken);
    }

    [Fact]
    public async Task OnHandle_WhenIuuNotificationWithSameUpdatedTimeReceived_ThenIuuNotificationIsNotCreatedOrUpdated()
    {
        var iuuReference = "IUU.FR.2026.0000001";
        var lastUpdatedTimestamp = new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc);

        var existingIuuNotification = new DataApiIuu.IuuNotification
        {
            Reference = iuuReference,
            LastUpdated = lastUpdatedTimestamp,
            IuuOption = "IUUOK",
        };

        _mockApi
            .GetIuuNotification(iuuReference, _cancellationToken)
            .Returns(
                new IuuNotificationResponse(
                    existingIuuNotification,
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 10, 19, 9, 0, 0, DateTimeKind.Utc),
                    "etag"
                )
            );

        var iuuNotification = IuuNotificationFixture(lastUpdated: lastUpdatedTimestamp).Create();

        await _subject.OnHandle(iuuNotification, _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutIuuNotification(iuuReference, Arg.Any<DataApiIuu.IuuNotification>(), null, _cancellationToken);

        await _mockApi
            .DidNotReceive()
            .PutIuuNotification(iuuReference, Arg.Any<DataApiIuu.IuuNotification>(), "etag", _cancellationToken);
    }
}
