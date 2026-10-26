using BtmsGateway.Contract.CustomsDeclarations;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions.Model;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class InboundErrorTests
{
    [Fact]
    public async Task InboundError_ConversionToDataApiInboundError_IsCorrect()
    {
        var inboundError = new InboundError
        {
            Header = new InboundErrorHeader
            {
                EntryReference = "25GB98ONYJQZT5TAR5",
                EntryVersionNumber = 1,
                SourceCorrelationId = "54321",
            },
            ServiceHeader = new ServiceHeader
            {
                CorrelationId = "12345",
                DestinationSystem = "ALVS",
                ServiceCallTimestamp = new DateTime(2025, 04, 15, 12, 0, 0, DateTimeKind.Utc),
                SourceSystem = "CDS",
            },
            Errors = [new InboundErrorItem { ErrorCode = "T04ST", ErrorMessage = "It has been overcooked." }],
        };

        var dataApiInboundError = inboundError.ToDataApiExternalError();

        await Verify(dataApiInboundError).DontScrubDateTimes();
    }
}
