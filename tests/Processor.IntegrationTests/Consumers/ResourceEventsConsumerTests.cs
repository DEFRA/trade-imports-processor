using System.Text.Json;
using Amazon.SQS.Model;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsDataApi.Domain.Events;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.Clients;
using Defra.TradeImportsProcessor.Processor.IntegrationTests.TestBase;
using FluentAssertions;
using Xunit.Abstractions;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.IntegrationTests.Consumers;

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToUpper();
}

[Collection("UsesServiceBus")]
public class ResourceEventsConsumerTests(ServiceBusFixture serviceBusFixture, ITestOutputHelper output)
    : SqsTestBase(output)
{
    private readonly string MRN = "25GB001ABCDEF1ABC5";
    private readonly ServiceBusFixtureClient _serviceBusFixtureClient = serviceBusFixture.GetClient(
        "alvs_topic",
        "trade-imports-processor.tests.subscription"
    );
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = new UpperCaseNamingPolicy(),
    };

    [Fact]
    public async Task WhenDecisionNotificationSent_ThenDecisionNotificationIsSentToAlvsServiceBusTopic()
    {
        var customsDeclaration = new CustomsDeclarationEvent
        {
            Id = "test",
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

        var resourceEvent = new ResourceEvent<CustomsDeclarationEvent>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceDecision",
            Operation = "Created",
            Etag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent, _jsonSerializerOptions),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes<ResourceEvent<CustomsDeclarationEvent>>(
                "CustomsDeclaration",
                "ClearanceDecision",
                MRN
            ),
            false
        );

        var messages = await _serviceBusFixtureClient.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("ALVSDecisionNotification");
        messages[0].ApplicationProperties["subType"].Should().Be("ALVS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
    }

    [Fact]
    public async Task WhenClearanceRequestSent_ThenClearanceRequestIsSentToAlvsServiceBusTopic()
    {
        var customsDeclaration = new CustomsDeclarationEvent
        {
            Id = "test",
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

        var resourceEvent = new ResourceEvent<CustomsDeclarationEvent>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "ClearanceRequest",
            Operation = "Created",
            Etag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes<ResourceEvent<CustomsDeclarationEvent>>(
                "CustomsDeclaration",
                "ClearanceRequest",
                MRN
            ),
            false
        );

        var messages = await _serviceBusFixtureClient.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("ALVSClearanceRequest");
        messages[0].ApplicationProperties["subType"].Should().Be("CDS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
    }

    [Fact]
    public async Task WhenFinalisationSent_ThenFinalisationIsSentToAlvsServiceBusTopic()
    {
        var customsDeclaration = new CustomsDeclarationEvent
        {
            Id = "test",
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

        var resourceEvent = new ResourceEvent<CustomsDeclarationEvent>
        {
            ResourceId = MRN,
            ResourceType = "CustomsDeclaration",
            SubResourceType = "Finalisation",
            Operation = "Created",
            Etag = "123",
            Resource = customsDeclaration,
        };

        await SendMessage(
            MRN,
            JsonSerializer.Serialize(resourceEvent),
            ResourceEventsQueueUrl,
            WithResourceEventAttributes<ResourceEvent<CustomsDeclarationEvent>>(
                "CustomsDeclaration",
                "Finalisation",
                MRN
            ),
            false
        );

        var messages = await _serviceBusFixtureClient.Receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));

        messages.Count.Should().Be(1);
        messages[0].ApplicationProperties.Count.Should().Be(2);
        messages[0].ApplicationProperties["messageType"].Should().Be("FinalisationNotificationRequest");
        messages[0].ApplicationProperties["subType"].Should().Be("CDS");

        await VerifyJson(messages[0].Body.ToString()).DontIgnoreEmptyCollections().UseStrictJson();
    }
}
