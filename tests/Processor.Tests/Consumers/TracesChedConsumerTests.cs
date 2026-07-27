using AutoFixture;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Trade.Gateway.Api.Contract.Certificate;
using static Defra.TradeImportsProcessor.TestFixtures.ImportNotificationFixtures;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class TracesChedConsumerTests
{
    private const string ExpectedEtag = "12345";
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly ITradeImportsDataApiClient _mockApi = Substitute.For<ITradeImportsDataApiClient>();
    private readonly ILogger<TracesChedConsumer> _mockLogger = Substitute.For<ILogger<TracesChedConsumer>>();

    [Fact]
    public async Task OnHandle_WhenTracesChedReceived_AndNoTracesChedExistsInTheDataApi_ThenItIsCreated()
    {
        var consumer = new TracesChedConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();
        var ched = new DefraUNVTDCHEDProfile
        {
            ExchangedDocument = new ExchangedDocument()
            {
                Identifier = importNotification.ReferenceNumber,
                NotificationStatusCode = importNotification.Status,
                IncludedNote =
                [
                    new IncludedNote()
                    {
                        Content = [DateTime.UtcNow.ToString("o")],
                        SubjectCode = new CodedValue() { Value = "LAST_UPDATE_DATETIME" },
                    },
                ],
            },
            SpecifiedConsignment = new Consignment(),
        };

        _mockApi
            .GetTracesChed(importNotification.ReferenceNumber, _cancellationToken)
            .Returns(null as TracesChedResponse);

        await consumer.OnHandle(ched, _cancellationToken);

        await _mockApi
            .Received()
            .PutTracesChed(
                importNotification.ReferenceNumber,
                Arg.Any<DefraUNVTDCHEDProfile>(),
                null,
                _cancellationToken
            );
    }

    [Fact]
    public async Task OnHandle_WhenTracesChedReceived_AndOneAlreadyExistsInTheDataApi_ThenItIsUpdated()
    {
        var consumer = new TracesChedConsumer(_mockLogger, _mockApi);

        var importNotification = ImportNotificationFixture().Create();

        var ched = new DefraUNVTDCHEDProfile
        {
            ExchangedDocument = new ExchangedDocument()
            {
                Identifier = importNotification.ReferenceNumber,
                NotificationStatusCode = importNotification.Status,
                IncludedNote =
                [
                    new IncludedNote()
                    {
                        Content = [DateTime.UtcNow.ToString("o")],
                        SubjectCode = new CodedValue() { Value = "LAST_UPDATE_DATETIME" },
                    },
                ],
            },
            SpecifiedConsignment = new Consignment(),
        };

        var response = new TracesChedResponse(ched, DateTime.Now, DateTime.Now, ExpectedEtag);

        _mockApi.GetTracesChed(importNotification.ReferenceNumber, _cancellationToken).Returns(response);

        ched = new DefraUNVTDCHEDProfile
        {
            ExchangedDocument = new ExchangedDocument()
            {
                Identifier = importNotification.ReferenceNumber,
                NotificationStatusCode = importNotification.Status,
                IncludedNote =
                [
                    new IncludedNote()
                    {
                        Content = [DateTime.UtcNow.ToString("o")],
                        SubjectCode = new CodedValue() { Value = "LAST_UPDATE_DATETIME" },
                    },
                ],
            },
            SpecifiedConsignment = new Consignment(),
        };

        await consumer.OnHandle(ched, _cancellationToken);

        await _mockApi
            .Received()
            .PutTracesChed(
                importNotification.ReferenceNumber,
                Arg.Any<DefraUNVTDCHEDProfile>(),
                ExpectedEtag,
                _cancellationToken
            );
    }
}
