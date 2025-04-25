using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class InboundErrorTests
{
    [Fact]
    public async Task InboundError_ConversionToDataApiInboundError_IsCorrect()
    {
        var inboundError = new InboundError
        {
            Header = new Header { EntryReference = "25GB98ONYJQZT5TAR5", EntryVersionNumber = 1 },
            ServiceHeader = new ServiceHeader
            {
                CorrelationId = "12345",
                DestinationSystem = "ALVS",
                ServiceCallTimestamp = new DateTime(2025, 04, 15, 12, 0, 0, DateTimeKind.Utc),
                SourceSystem = "CDS",
            },
            Errors = [new InboundErrorItem { errorCode = "T04ST", errorMessage = "It has been overcooked." }],
        };

        var dataApiInboundError = (DataApiCustomsDeclaration.InboundErrorNotification)inboundError;

        await Verify(dataApiInboundError).DontScrubDateTimes();
    }
}
