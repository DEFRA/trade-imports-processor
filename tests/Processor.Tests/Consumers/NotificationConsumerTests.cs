using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
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

        var importNotification = ImportNotificationFixture();
        var cancellationToken = CancellationToken.None;

        _mockApi
            .GetImportNotification(importNotification.ReferenceNumber!, cancellationToken)
            .Returns(null as ImportPreNotificationResponse);

        await consumer.OnHandle(importNotification, cancellationToken);

        await _mockApi
            .Received()
            .PutImportNotification(
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

        var importNotification = ImportNotificationFixture();
        var dataApiImportNotification = DataApiImportNotificationFixture();
        var cancellationToken = CancellationToken.None;
        const string expectedEtag = "12345";
        var response = new ImportPreNotificationResponse(
            dataApiImportNotification,
            DateTime.Now,
            DateTime.Now,
            expectedEtag
        );

        _mockApi.GetImportNotification(importNotification.ReferenceNumber!, cancellationToken).Returns(response);

        await consumer.OnHandle(importNotification, cancellationToken);

        await _mockApi
            .Received()
            .PutImportNotification(
                importNotification.ReferenceNumber!,
                Arg.Any<IpaffsDataApi.ImportPreNotification>(),
                expectedEtag,
                cancellationToken
            );
    }
}
