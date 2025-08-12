using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlimMessageBus;
using CustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class IpaffsConsumerTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IOptions<BtmsOptions> btmsOptions = Substitute.For<IOptions<BtmsOptions>>();
    private readonly IIpaffsStrategyFactory ipaffsStrategyFactory = Substitute.For<IIpaffsStrategyFactory>();
    private readonly IIpaffsStrategy ipaffsStrategy = Substitute.For<IIpaffsStrategy>();

    private readonly IpaffsConsumer _ipaffsConsumer;

    public IpaffsConsumerTests()
    {
        _ipaffsConsumer = new IpaffsConsumer(btmsOptions, ipaffsStrategyFactory);
        _ipaffsConsumer.Context = Substitute.For<IConsumerContext>();
    }

    [Fact]
    public async Task WhenInCutoverAndValidDecisionReceived_ThenMessagePublished()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });
        ipaffsStrategyFactory
            .TryGetIpaffsStrategy(Arg.Any<string>(), out Arg.Any<IIpaffsStrategy?>())
            .Returns(x =>
            {
                x[1] = ipaffsStrategy;
                return true;
            });

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
        _ipaffsConsumer.Context.Headers.Returns(headers);
        var transportMessage = new Message { MessageId = "SQS123" };
        var messageProperties = new Dictionary<string, object> { ["Sqs_Message"] = transportMessage };
        _ipaffsConsumer.Context.Properties.Returns(messageProperties);

        await _ipaffsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await ipaffsStrategy
            .Received()
            .PublishToIpaffsAsync(
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CustomsDeclaration?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenNotInCutover_ThenMessageIsNotPublished()
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

        await _ipaffsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffsAsync(
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CustomsDeclaration?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenInCutoverAndResourceEventIsNotCustomsDeclaration_ThenMessageIsNotPublished()
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
        _ipaffsConsumer.Context.Headers.Returns(headers);

        await _ipaffsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffsAsync(
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CustomsDeclaration?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenInCutoverAndSubResourceTypeHasNoStrategy_ThenMessageIsNotPublishedToAzureTopic()
    {
        btmsOptions.Value.Returns(new BtmsOptions { OperatingMode = OperatingMode.Cutover });
        ipaffsStrategyFactory.TryGetIpaffsStrategy(Arg.Any<string>(), out Arg.Any<IIpaffsStrategy?>()).Returns(false);

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
        _ipaffsConsumer.Context.Headers.Returns(headers);

        await _ipaffsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffsAsync(
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CustomsDeclaration?>(),
                Arg.Any<CancellationToken>()
            );
    }
}
