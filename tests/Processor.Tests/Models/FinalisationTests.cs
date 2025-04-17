using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class FinalisationTests
{
    [Fact]
    public async Task Finalisation_ConversionToDataApiFinalisation_IsCorrect()
    {
        var finalisation = new Finalisation
        {
            Header = new FinalisationHeader
            {
                EntryReference = "25GB98ONYJQZT5TAR5",
                EntryVersionNumber = 1,
                DecisionNumber = 1,
                FinalState = "1",
                ManualAction = "Y",
            },
            ServiceHeader = new ServiceHeader
            {
                CorrelationId = "12345",
                DestinationSystem = "ALVS",
                ServiceCallTimestamp = new DateTime(2025, 04, 15, 12, 0, 0, DateTimeKind.Utc),
                SourceSystem = "CDS",
            },
        };

        var dataApiFinalisation = (DataApiCustomsDeclaration.Finalisation)finalisation;

        await Verify(dataApiFinalisation).DontScrubDateTimes();
    }
}
