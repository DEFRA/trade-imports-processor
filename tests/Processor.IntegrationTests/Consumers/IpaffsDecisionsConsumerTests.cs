using System.Text.Json;
using Amazon.SQS.Model;
using Azure.Messaging.ServiceBus;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using Xunit.Abstractions;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

public class IpaffsDecisionsConsumerTests(ITestOutputHelper output) : SqsTestBase(output)
{
    private readonly ServiceBusHandler ServiceBus = new();

    [Fact]
    public async Task WhenDecisionNotificationSent_ThenDecisionNotificationIsSentToAlvsServiceBusTopic()
    {
        await ServiceBus.InitializeAsync();

        var mrn = "25GB001ABCDEF1ABC5";
        var customsDeclaration = new CustomsDeclaration()
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
            ResourceId = mrn,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            mrn,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes("CustomsDeclaration", "ClearanceDecision", mrn),
            false
        );

        var messages = await ServiceBus.GetMessagesAsync();

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties["messageType"].Should().Be("ALVSDecisionNotification");
        messages[0].ApplicationProperties["subType"].Should().Be("ALVS");

        await VerifyJson(messages[0].Body.ToString());
    }

    private static Dictionary<string, MessageAttributeValue> WithResourceEventAttributes(
        string resourceType,
        string subResourceType,
        string resourceId
    )
    {
        return new Dictionary<string, MessageAttributeValue>
        {
            {
                MessageBusHeaders.ResourceTypeHeader,
                new MessageAttributeValue { DataType = "String", StringValue = resourceType }
            },
            {
                MessageBusHeaders.SubResourceTypeHeader,
                new MessageAttributeValue { DataType = "String", StringValue = subResourceType }
            },
            {
                MessageBusHeaders.ResourceId,
                new MessageAttributeValue { DataType = "String", StringValue = resourceId }
            },
        };
    }

    private class ServiceBusHandler() : ServiceBusTestBase("alvs_topic", "trade-imports-processor.tests.subscription")
    {
        public async Task<IReadOnlyList<ServiceBusReceivedMessage>> GetMessagesAsync()
        {
            return await Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));
        }
    }
}
