using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

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
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndOneAlreadyExistsInTheDataApi_ThenItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();
        var dataApiImportNotification = DataApiImportNotificationFixture().Create();
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
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Theory]
    [InlineData(ImportNotificationStatus.Cancelled)]
    [InlineData(ImportNotificationStatus.Deleted)]
    public async Task OnHandle_WhenTheImportNotificationIsDeletedOrCancelled_ItShouldStillBeProcessed(
        ImportNotificationStatus status
    )
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().With(i => i.Status, status).Create();

        await consumer.OnHandle(JsonSerializer.SerializeToElement(importNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber,
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                null,
                _cancellationToken
            );
    }

    [Theory]
    [InlineData(ImportNotificationStatus.Amend)]
    [InlineData(ImportNotificationStatus.Draft)]
    public async Task OnHandle_WhenImportNotificationShouldNotBeProcessed_ThenItIsSkipped(
        ImportNotificationStatus status
    )
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
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
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
        var existingNotification = DataApiImportNotificationFixture().With(i => i.UpdatedSource, DateTime.Now).Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, "1"));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Theory]
    [InlineData(IpaffsDataApi.ImportNotificationStatus.Validated)]
    [InlineData(IpaffsDataApi.ImportNotificationStatus.Rejected)]
    [InlineData(IpaffsDataApi.ImportNotificationStatus.PartiallyRejected)]
    public async Task OnHandle_WhenNewImportNotificationIsInProgress_AndTheExistingIsMoreMature_Skip(
        IpaffsDataApi.ImportNotificationStatus existingStatus
    )
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);
        var newNotification = ImportNotificationFixture()
            .With(i => i.Status, ImportNotificationStatus.InProgress)
            .Create();

        var existingNotification = DataApiImportNotificationFixture().With(i => i.Status, existingStatus).Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, "1"));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .DidNotReceiveWithAnyArgs()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
