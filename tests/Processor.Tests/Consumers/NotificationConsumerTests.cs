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
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly ILogger<NotificationConsumer> _mockLogger = Substitute.For<ILogger<NotificationConsumer>>();

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndNoImportNotificationExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();
        var cancellationToken = CancellationToken.None;

        _mockApi
            .GetImportPreNotification(importNotification.ReferenceNumber!, cancellationToken)
            .Returns(null as ImportPreNotificationResponse);

        await consumer.OnHandle(importNotification, cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber!,
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                null,
                cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenImportNotificationReceived_AndOneAlreadyExistsInTheDataApi_ThenItIsUpdated()
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();
        var dataApiImportNotification = DataApiImportNotificationFixture().Create();
        var cancellationToken = CancellationToken.None;
        const string expectedEtag = "12345";
        var response = new ImportPreNotificationResponse(
            dataApiImportNotification,
            DateTime.Now,
            DateTime.Now,
            expectedEtag
        );

        _mockApi.GetImportPreNotification(importNotification.ReferenceNumber!, cancellationToken).Returns(response);

        await consumer.OnHandle(importNotification, cancellationToken);

        await _mockApi
            .Received()
            .PutImportPreNotification(
                importNotification.ReferenceNumber!,
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                expectedEtag,
                cancellationToken
            );
    }

    [Theory]
    [InlineData(ImportNotificationStatus.Amend)]
    [InlineData(ImportNotificationStatus.Draft)]
    public async Task OnHandle_WhenImportNotificationIsShouldNotBeProcessed_ThenItIsSkipped(
        ImportNotificationStatus status
    )
    {
        var consumer = new NotificationConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().With(i => i.Status, status).Create();
        var cancellationToken = CancellationToken.None;

        await consumer.OnHandle(importNotification, cancellationToken);

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
}
