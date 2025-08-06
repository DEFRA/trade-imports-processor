using System.Text.Json;
using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class NotificationConsumerTests
{
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly ILogger<NotificationConsumer> _mockLogger = Substitute.For<ILogger<NotificationConsumer>>();

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndDoesNotDeserialize_ThenItThrows()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var act = async () => await consumer.OnHandle(JsonDocument.Parse("null").RootElement, _cancellationToken);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .And.Message.Should()
            .Be("Received invalid message, deserialised as null");
    }

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

        var importNotification = ImportNotificationFixture()
            .With(i => i.Status, ImportNotificationStatus.InProgress)
            .Create();
        var dataApiImportNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, DateTime.UtcNow.AddMinutes(-5))
                .With(i => i.Status, ImportNotificationStatus.InProgress)
                .Create();
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

    [Fact]
    public async Task OnHandle_WhenNewImportNotificationIsValidated_AndTheExistingIsInProgress_ButHasTheSameTimestamp_ThenItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);
        var existingNotificationTimestamp = DateTime.UtcNow;
        var newNotificationTimestamp = DateTime.UtcNow.AddMicroseconds(-1);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.Validated)
            .Create();

        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, ImportNotificationStatus.InProgress)
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                newNotification.ReferenceNumber,
                Arg.Is<DataApiIpaffs.ImportPreNotification>(i => i.Status == ImportNotificationStatus.Validated),
                ExpectedEtag,
                _cancellationToken
            );
    }

    [Theory]
    [InlineData(ImportNotificationStatus.Cancelled)]
    [InlineData(ImportNotificationStatus.Deleted)]
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
    [InlineData(ImportNotificationStatus.Draft)]
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

        var existingNotificationTimestamp = DateTime.UtcNow;
        var newNotificationTimestamp = DateTime.UtcNow.AddMilliseconds(-1);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.InProgress)
            .Create();
        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, ImportNotificationStatus.InProgress)
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

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

    [Fact]
    public async Task OnHandle_WhenNewImportNotificationMovingFromSubmittedToValidated_ItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var existingNotificationTimestamp = new DateTime(2025, 05, 05, 10, 07, 33, 817, DateTimeKind.Utc);
        var newNotificationTimestamp = new DateTime(2025, 05, 07, 13, 54, 23, 984, DateTimeKind.Utc);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.Validated)
            .Create();
        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, "SUBMITTED")
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Is<DataApiIpaffs.ImportPreNotification>(n => n.Status == ImportNotificationStatus.Validated),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task OnHandle_WhenNewImportNotificationMovingFromSubmittedToValidated_WithAnOlderTimestamp_ItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var existingNotificationTimestamp = new DateTime(2025, 05, 07, 10, 07, 33, 817, DateTimeKind.Utc);
        var newNotificationTimestamp = new DateTime(2025, 05, 07, 10, 07, 33, 111, DateTimeKind.Utc);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.Validated)
            .Create();
        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, "SUBMITTED")
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Is<DataApiIpaffs.ImportPreNotification>(n => n.Status == ImportNotificationStatus.Validated),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Theory]
    [InlineData(ImportNotificationStatus.Validated)]
    [InlineData(ImportNotificationStatus.Rejected)]
    [InlineData(ImportNotificationStatus.PartiallyRejected)]
    public async Task OnHandle_WhenNewImportNotificationIsInProgress_AndTheExistingIsMoreMature_Skip(
        string existingStatus
    )
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);
        var existingNotificationTimestamp = DateTime.UtcNow;
        var newNotificationTimestamp = DateTime.UtcNow.AddMicroseconds(-1);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.InProgress)
            .Create();

        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, existingStatus)
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

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

    [Fact]
    public async Task OnHandle_WhenImportNotificationMovingFromInProgressToAmend_ItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);
        var existingNotificationTimestamp = DateTime.UtcNow;
        var newNotificationTimestamp = DateTime.UtcNow.AddSeconds(5);

        var newNotification = ImportNotificationFixture()
            .With(i => i.LastUpdated, newNotificationTimestamp)
            .With(i => i.Status, ImportNotificationStatus.Amend)
            .Create();
        var existingNotification = (DataApiIpaffs.ImportPreNotification)
            ImportNotificationFixture()
                .With(i => i.LastUpdated, existingNotificationTimestamp)
                .With(i => i.Status, ImportNotificationStatus.InProgress)
                .Create();

        _mockApi
            .GetImportPreNotification(newNotification.ReferenceNumber, _cancellationToken)
            .Returns(new ImportPreNotificationResponse(existingNotification, DateTime.Now, DateTime.Now, ExpectedEtag));

        await consumer.OnHandle(JsonSerializer.SerializeToElement(newNotification), _cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                Arg.Any<string>(),
                Arg.Is<DataApiIpaffs.ImportPreNotification>(n => n.Status == ImportNotificationStatus.Amend),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
    }
}
