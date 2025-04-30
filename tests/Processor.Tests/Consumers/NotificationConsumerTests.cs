using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;
using DataApiIpaffsConstants = Defra.TradeImportsDataApi.Domain.Ipaffs.Constants;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class NotificationConsumerTests
{
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly ILogger<NotificationConsumer> _mockLogger = Substitute.For<ILogger<NotificationConsumer>>();

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndNoImportNotificationExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();

        _mockApi
            .GetImportPreNotification(importNotification.ReferenceNumber, _cancellationToken)
            .Returns(null as ImportPreNotificationResponse);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(importNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber,
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndOneAlreadyExistsInTheDataApi_ThenItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();
        var dataApiImportNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture().With(i => i.LastUpdated, DateTime.UtcNow.AddMinutes(-5)).Create();
        var response = new ImportPreNotificationResponse(
            dataApiImportNotification,
            DateTime.Now,
            DateTime.Now,
            ExpectedEtag
        );

        _mockApi.GetImportPreNotification(importNotification.ReferenceNumber, _cancellationToken).Returns(response);

        await consumer.OnHandle(JsonSerializer.SerializeToElement(importNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber,
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Theory]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Cancelled)]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Deleted)]
    public async Task OnHandle_WhenTheImportNotificationIsDeletedOrCancelled_ItShouldStillBeProcessed(string status)
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().With(i => i.Status, status).Create();

        await consumer.OnHandle(JsonSerializer.SerializeToElement(importNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber,
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                null,
                _cancellationToken
            );
    }

    [Theory]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Amend)]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Draft)]
    public async Task OnHandle_WhenImportNotificationShouldNotBeProcessed_ThenItIsSkipped(string status)
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().With(i => i.Status, status).Create();

        await consumer.OnHandle(JsonSerializer.SerializeToElement(importNotification), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .GetImportPreNotification(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task OnHandle_WhenNewImportNotificationIsOlderThanExistingNotification_Skip()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, DateTime.Now.AddSeconds(-5))
            .Create();
        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture().With(i => i.LastUpdated, DateTime.Now).Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, "1"));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Theory]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Validated)]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.Rejected)]
    [InlineData(DataApiIpaffsConstants.ImportNotificationStatus.PartiallyRejected)]
    public async Task OnHandle_WhenNewImportNotificationIsInProgress_AndTheExistingIsMoreMature_Skip(
        string existingStatus
    )
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);
        var newNotification = ImportNotificationFixture()
            .With(i => i.Status, DataApiIpaffsConstants.ImportNotificationStatus.InProgress)
            .Create();

        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture().With(i => i.Status, existingStatus).Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, "1"));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Any<DataApiIpaffs.ImportPreNotification>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
