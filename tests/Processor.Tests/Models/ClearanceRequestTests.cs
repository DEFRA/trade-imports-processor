using Defra.TradeImportsProcessor.Processor.Consumers;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using Microsoft.Extensions.Logging.Testing;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class ClearanceRequestTests
{
    [Theory]
    [InlineData("Valid", "GBCHD2025.3338265", false)]
    [InlineData("SpaceAtEnd", "GBCHD2025.3338265 ", true)]
    [InlineData("Carriage Return", "GBCHD2025.3338265\r\n", true)]
    [InlineData("Tab", "GBCHD2025.3338265\t", true)]
    public async Task ClearanceRequest_ConversionToDataApiClearanceRequest_IsCorrect(
        string fileName,
        string documentReference,
        bool expectedLog
    )
    {
        var clearanceRequest = new ClearanceRequest
        {
            ServiceHeader = new ServiceHeader
            {
                CorrelationId = "12345",
                DestinationSystem = "ALVS",
                ServiceCallTimestamp = new DateTime(2025, 04, 15, 12, 0, 0, DateTimeKind.Utc),
                SourceSystem = "CDS",
            },
            Header = new ClearanceRequestHeader
            {
                EntryReference = "25GB98ONYJQZT5TAR5",
                EntryVersionNumber = 1,
                PreviousVersionNumber = null,
                DeclarationUcr = "9GB311241319000-MOR-009867",
                DeclarationPartNumber = null,
                DeclarationType = "S",
                ArrivalDateTime = null,
                SubmitterTurn = "GB391241319001",
                DeclarantId = "GB391241319001",
                DeclarantName = "GB391241319001",
                DispatchCountryCode = "MA",
                GoodsLocationCode = "DVRDOGFSB",
                MasterUcr = "MOOB74211124478",
            },
            Items =
            [
                new Item
                {
                    ItemNumber = 1,
                    CustomsProcedureCode = "4000000",
                    TaricCommodityCode = "0702000099",
                    GoodsDescription = "Loads of sausages",
                    ConsigneeId = "GB391241319002",
                    ConsigneeName = "Tim Sausage",
                    ItemNetMass = 5,
                    ItemSupplementaryUnits = 0,
                    ItemThirdQuantity = null,
                    ItemOriginCountryCode = "MA",
                    Documents =
                    [
                        new Document
                        {
                            DocumentCode = "N002",
                            DocumentReference = documentReference,
                            DocumentStatus = "AG",
                            DocumentControl = "P",
                            DocumentQuantity = null,
                        },
                    ],
                    Checks = [new Check { CheckCode = "H218", DepartmentCode = "HMI" }],
                },
            ],
        };

        var dataApiClearanceRequest = (DataApiCustomsDeclaration.ClearanceRequest)clearanceRequest;

        var logger = new FakeLogger();
        CustomsDeclarationsConsumer.LogAnyDocumentReferenceWithWhitespaceSuffix(clearanceRequest, logger);

        logger.Collector.Count.Should().Be(expectedLog ? 1 : 0);

        await Verify(dataApiClearanceRequest)
            .UseFileName(
                $"{nameof(ClearanceRequestTests)}.{nameof(ClearanceRequest_ConversionToDataApiClearanceRequest_IsCorrect)}_{fileName}"
            )
            .DontScrubDateTimes();
    }
}
