using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using Defra.TradeImportsProcessor.Processor.Models.Ipaffs;
using Defra.TradeImportsProcessor.Processor.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SlimMessageBus;
using ClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;
using IpaffsClearanceRequest = Defra.TradeImportsProcessor.Processor.Models.Ipaffs.ClearanceRequest;

namespace Defra.TradeImportsProcessor.Processor.Tests.Services;

public class ClearanceRequestStrategyTests
{
    private const string Mrn = "25GB001ABCDEF1ABC5";

    private readonly IMessageBus azureServiceBus = Substitute.For<IMessageBus>();
    private readonly ILogger<ClearanceRequestStrategy> logger = Substitute.For<ILogger<ClearanceRequestStrategy>>();

    private readonly ClearanceRequestStrategy clearanceRequestStrategy;

    public ClearanceRequestStrategyTests()
    {
        clearanceRequestStrategy = new ClearanceRequestStrategy(azureServiceBus, logger);
    }

    [Fact]
    public async Task WhenValidClearanceRequestReceived_ThenMessagePublishedToAzureTopic()
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
                        NetMass = 1,
                        SupplementaryUnits = 1,
                        ThirdQuantity = 1,
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

        await clearanceRequestStrategy.PublishToIpaffs("SQS123", Mrn, customsDeclaration, CancellationToken.None);

        await azureServiceBus
            .Received()
            .Publish(
                Arg.Is<IpaffsClearanceRequest>(x => x.Header.EntryReference == Mrn),
                Arg.Is<string>(x => x == null),
                Arg.Is<Dictionary<string, object>>(e =>
                    e["messageType"].ToString() == "ALVSClearanceRequest" && e["subType"].ToString() == "CDS"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task WhenClearanceRequestIsNull_ThenExceptionIsThrown()
    {
        await Assert.ThrowsAsync<ResourceEventException>(() =>
            clearanceRequestStrategy.PublishToIpaffs("SQS123", Mrn, new CustomsDeclaration(), CancellationToken.None)
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
