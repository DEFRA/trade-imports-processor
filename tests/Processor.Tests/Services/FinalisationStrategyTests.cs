using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;
using IpaffsFinalisation = Defra.TradeImportsProcessor.Processor.Models.Ipaffs.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Tests.Services;

public class FinalisationStrategyTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IMessageBus azureServiceBus = Substitute.For<IMessageBus>();
    private readonly ILogger<FinalisationStrategy> logger = Substitute.For<ILogger<FinalisationStrategy>>();

    private readonly FinalisationStrategy finalisationStrategy;

    public FinalisationStrategyTests()
    {
        finalisationStrategy = new FinalisationStrategy(azureServiceBus, logger);
    }

    [Fact]
    public async Task WhenValidFinalisationReceived_ThenMessagePublishedToAzureTopic()
    {
        var customsDeclaration = new CustomsDeclarationEvent
        {
            Id = "test",
            Finalisation = new Finalisation
            {
                DecisionNumber = 1,
                ExternalCorrelationId = "ABC123",
                ExternalVersion = 1,
                FinalState = "0",
                IsManualRelease = true,
                MessageSentAt = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            },
        };

        await finalisationStrategy.PublishToIpaffs("SQS123", Mrn, customsDeclaration, CancellationToken.None);

        await azureServiceBus
            .Received()
            .Publish(
                Arg.Is<IpaffsFinalisation>(x =>
                    x.ServiceHeader.SourceSystem == "CDS"
                    && x.ServiceHeader.DestinationSystem == "ALVS"
                    && x.ServiceHeader.CorrelationId == customsDeclaration.Finalisation.ExternalCorrelationId
                    && x.ServiceHeader.ServiceCallTimestamp == customsDeclaration.Finalisation.MessageSentAt
                    && x.Header.EntryReference == Mrn
                    && x.Header.EntryVersionNumber == customsDeclaration.Finalisation.ExternalVersion
                    && x.Header.DecisionNumber == customsDeclaration.Finalisation.DecisionNumber
                    && x.Header.FinalState == customsDeclaration.Finalisation.FinalState
                    && x.Header.ManualAction == "Y"
                ),
                Arg.Is<string>(x => x == null),
                Arg.Is<Dictionary<string, object>>(e =>
                    e["messageType"].ToString() == "FinalisationNotificationRequest" && e["subType"].ToString() == "CDS"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenFinalisationIsNull_ThenExceptionIsThrown()
    {
        await Assert.ThrowsAsync<ResourceEventException>(() =>
            finalisationStrategy.PublishToIpaffs(
                "SQS123",
                Mrn,
                new CustomsDeclarationEvent() { Id = "Test" },
                CancellationToken.None
            )
        );

        await azureServiceBus
            .DidNotReceive()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            );
    }
}
