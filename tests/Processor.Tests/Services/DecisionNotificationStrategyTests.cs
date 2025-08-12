using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus;
using CustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Services;

public class DecisionNotificationStrategyTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IMessageBus azureServiceBus = Substitute.For<IMessageBus>();
    private readonly ILogger<DecisionNotificationStrategy> logger = Substitute.For<
        ILogger<DecisionNotificationStrategy>
    >();

    private readonly DecisionNotificationStrategy decisionNotificationStrategy;

    public DecisionNotificationStrategyTests()
    {
        decisionNotificationStrategy = new DecisionNotificationStrategy(azureServiceBus, logger);
    }

    [Fact]
    public async Task WhenValidDecisionReceived_ThenMessagePublishedToAzureTopic()
    {
        var customsDeclaration = new CustomsDeclaration
        {
            ClearanceDecision = new ClearanceDecision
            {
                CorrelationId = "ABC123",
                DecisionNumber = 1,
                ExternalVersionNumber = 1,
                Items = new[]
                {
                    new ClearanceDecisionItem
                    {
                        ItemNumber = 1,
                        Checks = new[]
                        {
                            new ClearanceDecisionCheck
                            {
                                CheckCode = "H219",
                                DecisionCode = "X00",
                                DecisionsValidUntil = new DateTime(2025, 01, 08, 12, 0, 0, DateTimeKind.Utc),
                                DecisionReasons = new[] { "Test reason" },
                            },
                        },
                    },
                },
                Created = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            },
        };

        await decisionNotificationStrategy.PublishToIpaffs("SQS123", Mrn, customsDeclaration, CancellationToken.None);

        await azureServiceBus
            .Received()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Is<Dictionary<string, object>>(e =>
                    e["messageType"].ToString() == "ALVSDecisionNotification" && e["subType"].ToString() == "ALVS"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenClearanceDecisionIsNull_ThenExceptionIsThrown()
    {
        await Assert.ThrowsAsync<ResourceEventException>(() =>
            decisionNotificationStrategy.PublishToIpaffs(
                "SQS123",
                Mrn,
                new CustomsDeclaration(),
                CancellationToken.None
            )
        );

        await azureServiceBus
            .DidNotReceive()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Is<Dictionary<string, object>>(e =>
                    e["messageType"].ToString() == "ALVSDecisionNotification" && e["subType"].ToString() == "ALVS"
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
