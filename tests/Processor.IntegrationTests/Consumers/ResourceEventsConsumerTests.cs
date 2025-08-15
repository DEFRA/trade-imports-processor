using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using Xunit.Abstractions;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

[Collection("UsesServiceBus")]
public class ResourceEventsConsumerTests : SqsTestBase
{
    private readonly string MRN = "25GB001ABCDEF1ABC5";
    private readonly ServiceBusFixture _serviceBusFixture;

    public ResourceEventsConsumerTests(ServiceBusFixture serviceBusFixture, ITestOutputHelper output)
        : base(output)
    {
        _serviceBusFixture = serviceBusFixture;
        _serviceBusFixture.Using("alvs_topic", "trade-imports-processor.tests.subscription");
    }

    [Fact]
    public async Task WhenDecisionNotificationSent_ThenDecisionNotificationIsSentToAlvsServiceBusTopic()
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
                                DecisionReasons = new[] { "Test reason 1", "Test reason 2" },
                            },
                        },
                    },
                },
                Created = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            },
        };

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            ETag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes("CustomsDeclaration", "ClearanceDecision", MRN),
            false
        );

        var messages = await _serviceBusFixture.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("ALVSDecisionNotification");
        messages[0].ApplicationProperties["subType"].Should().Be("ALVS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
    }

    [Fact]
    public async Task WhenClearanceRequestSent_ThenClearanceRequestIsSentToAlvsServiceBusTopic()
    {
        var customsDeclaration = new CustomsDeclaration
        {
            ClearanceRequest = new ClearanceRequest
            {
                ExternalCorrelationId = "ABC123",
                MessageSentAt = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ExternalVersion = 2,
                PreviousExternalVersion = 1,
                DeclarationUcr = "TestUcr",
                DeclarationPartNumber = "TestPartNumber",
                DeclarationType = "TestType",
                ArrivesAt = new DateTime(2025, 01, 02, 12, 0, 0, DateTimeKind.Utc),
                SubmitterTurn = "TestSubmitterTurn",
                DeclarantId = "TestDeclarantId",
                DeclarantName = "TestDeclarantName",
                DispatchCountryCode = "TestDispatchCountryCode",
                GoodsLocationCode = "TestGoodsLocationCode",
                MasterUcr = "TestMasterUcr",
                Commodities = new[]
                {
                    new Commodity
                    {
                        ItemNumber = 1,
                        CustomsProcedureCode = "TestProcedureCode",
                        TaricCommodityCode = "TestTaricCommodityCode",
                        GoodsDescription = "TestGoodsDescription",
                        ConsigneeId = "TestConsigneeId",
                        ConsigneeName = "TestConsigneeName",
                        NetMass = 10,
                        SupplementaryUnits = 2,
                        ThirdQuantity = 3,
                        OriginCountryCode = "TestOriginCountryCode",
                        Documents =
                        [
                            new ImportDocument
                            {
                                DocumentCode = "TestDocumentCode",
                                DocumentReference = new ImportDocumentReference("TestDocumentReference"),
                                DocumentStatus = "TestDocumentStatus",
                                DocumentControl = "TestDocumentControl",
                                DocumentQuantity = 1,
                            },
                        ],
                        Checks =
                        [
                            new CommodityCheck { CheckCode = "TestCheckCode", DepartmentCode = "TestDepartmentCode" },
                        ],
                    },
                },
            },
        };

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceRequest",
            Operation = "Created",
            ETag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes("CustomsDeclaration", "ClearanceRequest", MRN),
            false
        );

        var messages = await _serviceBusFixture.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("ALVSClearanceRequest");
        messages[0].ApplicationProperties["subType"].Should().Be("CDS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
    }

    [Fact]
    public async Task WhenFinalisationSent_ThenFinalisationIsSentToAlvsServiceBusTopic()
    {
        var customsDeclaration = new CustomsDeclaration
        {
            Finalisation = new Finalisation
            {
                ExternalCorrelationId = "ABC123",
                MessageSentAt = new DateTime(2025, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ExternalVersion = 1,
                DecisionNumber = 1,
                FinalState = "0",
                IsManualRelease = true,
            },
        };

        var resourceEvent = new ResourceEvent<CustomsDeclaration>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Created",
            ETag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes("CustomsDeclaration", "Finalisation", MRN),
            false
        );

        var messages = await _serviceBusFixture.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("FinalisationNotificationRequest");
        messages[0].ApplicationProperties["subType"].Should().Be("CDS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
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
}
