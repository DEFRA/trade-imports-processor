using Defra.TradeImportsDataApi.Domain.Gvms;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;
using Gmr = Defra.TradeImportsProcessor.Processor.Models.Gmrs.Gmr;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class GmrTests
{
    [Fact]
    public async Task Gmr_ConversionToDataApiGmr_IsCorrect()
    {
        var timestamp = new DateTime(2025, 04, 15, 12, 0, 0, DateTimeKind.Utc);

        var gmr = new Gmr
        {
            GmrId = "GMRA2AMKTCIB",
            HaulierEori = "GB198906123655611",
            State = "OPEN",
            InspectionRequired = false,
            ReportToLocations = [new ReportToLocations { InspectionTypeId = "12345", LocationIds = ["12345"] }],
            UpdatedSource = timestamp,
            Direction = "UK_INBOUND",
            HaulierType = "FPO_ASN",
            IsUnaccompanied = true,
            VehicleRegistrationNumber = "XCR5WN",
            TrailerRegistrationNums = ["W8YK34", "P33TNV"],
            ContainerReferenceNums = ["ABCD", "EFGH"],
            PlannedCrossing = new PlannedCrossing { DepartsAt = timestamp, RouteId = "12345" },
            CheckedInCrossing = new CheckedInCrossing { ArrivesAt = timestamp, RouteId = "12345" },
            ActualCrossing = new ActualCrossing { ArrivesAt = timestamp, RouteId = "12345" },
            Declarations = new Declarations
            {
                Customs = [new Customs { Id = "12345" }],
                Transits = [new Transits { Id = "12345" }],
            },
        };

        var dataApiGmr = (DataApiGvms.Gmr)gmr;

        await Verify(dataApiGmr).DontScrubDateTimes();
    }
}
