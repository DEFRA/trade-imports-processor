using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsDataApi.Domain.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Configuration;
using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SlimMessageBus;
using CustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Consumers;

public class ResourceEventsConsumerTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IIpaffsStrategy _ipaffsStrategy = Substitute.For<IIpaffsStrategy>();
    private readonly ILogger<ResourceEventsConsumer> _logger = Substitute.For<ILogger<ResourceEventsConsumer>>();

    private ResourceEventsConsumer _resourceEventsConsumer;

    public ResourceEventsConsumerTests()
    {
        _ipaffsStrategy.SupportedSubResourceType.Returns("ClearanceDecision");
        _resourceEventsConsumer = new ResourceEventsConsumer([_ipaffsStrategy], _logger);
        _resourceEventsConsumer.Context = Substitute.For<IConsumerContext>();
        var transportMessage = new Message { MessageId = "SQS123" };
        var messageProperties = new Dictionary<string, object> { ["Sqs_Message"] = transportMessage };
        _resourceEventsConsumer.Context.Properties.Returns(messageProperties);
    }

    [Fact]
    public async Task WhenValidDecisionReceived_ThenMessagePublished()
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
        _resourceEventsConsumer.Context.Headers.Returns(headers);

        await _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await _ipaffsStrategy
            .Received()
            .PublishToIpaffs(
                Arg.Is("SQS123"),
                Arg.Is(Mrn),
                Arg.Is<CustomsDeclaration>(x =>
                    x.ClearanceDecision != null
                    && x.ClearanceDecision.CorrelationId == customsDeclaration.ClearanceDecision.CorrelationId
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenUnknownResourceType_ThenMessageIsNotPublished()
    {
        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = new CustomsDeclaration(),
        };

        await _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await _ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffs(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CustomsDeclaration>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenResourceEventIsNotCustomsDeclaration_ThenMessageIsNotPublished()
    {
        var resourceEvent = new ResourceEvent<ImportPreNotification>
        {
            ResourceId = Mrn,
            ResourceType = "ImportPreNotification",
            Operation = "Created",
            ETag = "123",
            Resource = new ImportPreNotification(),
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        _resourceEventsConsumer.Context.Headers.Returns(headers);

        await _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await _ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffs(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CustomsDeclaration>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenSubResourceTypeHasNoStrategy_ThenMessageIsNotPublishedToAzureTopic()
    {
        _resourceEventsConsumer = new ResourceEventsConsumer([], _logger);
        _resourceEventsConsumer.Context = Substitute.For<IConsumerContext>();

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
        _resourceEventsConsumer.Context.Headers.Returns(headers);
        var transportMessage = new Message { MessageId = "SQS123" };
        var messageProperties = new Dictionary<string, object> { ["Sqs_Message"] = transportMessage };
        _resourceEventsConsumer.Context.Properties.Returns(messageProperties);

        await _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None);

        await _ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffs(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CustomsDeclaration>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenResourceIdIsInvalid_ThenExceptionIsThrown()
    {
        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = string.Empty,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = new CustomsDeclaration(),
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        _resourceEventsConsumer.Context.Headers.Returns(headers);

        await Assert.ThrowsAsync<ResourceEventException>(() =>
            _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None)
        );

        await _ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffs(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CustomsDeclaration>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenResourceIsInvalid_ThenExceptionIsThrown()
    {
        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = Mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = null,
        };

        var headers = new Dictionary<string, object> { ["ResourceType"] = resourceEvent.ResourceType };
        _resourceEventsConsumer.Context.Headers.Returns(headers);

        await Assert.ThrowsAsync<ResourceEventException>(() =>
            _resourceEventsConsumer.OnHandle(JsonSerializer.Serialize(resourceEvent), CancellationToken.None)
        );

        await _ipaffsStrategy
            .DidNotReceive()
            .PublishToIpaffs(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CustomsDeclaration>(),
                Arg.Any<CancellationToken>()
            );
    }
}
