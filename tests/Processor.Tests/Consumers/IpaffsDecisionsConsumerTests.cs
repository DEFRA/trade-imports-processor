using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlimMessageBus;
using CustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class IpaffsDecisionsConsumerTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IOptions<BtmsOptions> btmsOptions = Substitute.For<IOptions<BtmsOptions>>();

    private readonly IMessageBus azureServiceBus = Substitute.For<IMessageBus>();
    private readonly ILogger<IpaffsDecisionsConsumer> logger = Substitute.For<ILogger<IpaffsDecisionsConsumer>>();

    private readonly IpaffsDecisionsConsumer ipaffsDecisionsConsumer;

    public IpaffsDecisionsConsumerTests()
    {
        ipaffsDecisionsConsumer = new IpaffsDecisionsConsumer(btmsOptions, azureServiceBus, logger);
        ipaffsDecisionsConsumer.Context = Substitute.For<IConsumerContext>();
    }

    [Fact]
    public async Task WhenInCutoverAndValidDecisionReceived_ThenMessagePublishedToAzureTopic()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });

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

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = customsDeclaration,
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        ipaffsDecisionsConsumer.Context.Headers.Returns(headers);

        await ipaffsDecisionsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

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
    public async Task WhenNotInCutover_ThenMessageIsNotPublishedToAzureTopic()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Default });

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = new CustomsDeclaration(),
        };

        await ipaffsDecisionsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await azureServiceBus
            .DidNotReceive()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenInCutoverAndResourceEventIsNotCustomsDeclaration_ThenMessageIsNotPublishedToAzureTopic()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });

        var resourceEvent = new ResourceEvent<ImportPreNotification>
        {
            ResourceId = Mrn,
            ResourceType = "ImportPreNotification",
            Operation = "Created",
            ETag = "123",
            Resource = new ImportPreNotification(),
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        ipaffsDecisionsConsumer.Context.Headers.Returns(headers);

        await ipaffsDecisionsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await azureServiceBus
            .DidNotReceive()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenInCutoverAndSubResourceTypeIsNotClearanceDecision_ThenMessageIsNotPublishedToAzureTopic()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Created",
            ETag = "123",
            Resource = new CustomsDeclaration(),
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        ipaffsDecisionsConsumer.Context.Headers.Returns(headers);

        await ipaffsDecisionsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await azureServiceBus
            .DidNotReceive()
            .Publish(
                Arg.Any<DecisionNotification>(),
                Arg.Any<string>(),
                Arg.Any<Dictionary<string, object>>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenInCutoverAndResourceIsNull_ThenExceptionThrown()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = new CustomsDeclaration { ClearanceDecision = null },
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        ipaffsDecisionsConsumer.Context.Headers.Returns(headers);
        var transportMessage = new Message { MessageId = "SQS123" };
        var messageProperties = new Dictionary<string, object> { ["Sqs_Message"] = transportMessage };
        ipaffsDecisionsConsumer.Context.Properties.Returns(messageProperties);

        await Assert.ThrowsAsync<ResourceEventException>(() =>
            ipaffsDecisionsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None)
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
