using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class ClearanceRequestTests
{
    [Fact]
    public async Task ClearanceRequest_ConversionToDataApiClearanceRequest_IsCorrect()
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
                            DocumentReference = "GBCHD2025.3338265",
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

        await Verify(dataApiClearanceRequest).DontScrubDateTimes();
    }
}
