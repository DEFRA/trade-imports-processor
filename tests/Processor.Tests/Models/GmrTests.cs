using Defra.TradeImportsDataApi.Domain.Gvms;
using Defra.TradeImportsProcessor.Processor.Models.Gmrs;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;
using Gmr = Defra.TradeImportsProcessor.Processor.Models.Gmrs.Gmr;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class GmrTests
{
    [Fact]
    public async Task Gmr_DeserializesCorrectly()
    {
        const string gmr = """
            {
              "GmrId": "GMRAADYA9J8G",
              "HaulierEori": "GB1196193155298",
              "State": "OPEN",
              "InspectionRequired": null,
              "ReportToLocations": null,
              "UpdatedDateTime": "2025-04-18T19:00:00.353Z",
              "Direction": "UK_INBOUND",
              "HaulierType": "NATO_MOD",
              "IsUnaccompanied": true,
              "VehicleRegNum": "RXPXOW",
              "TrailerRegistrationNums": [
                "7PIHPW",
                "E4FWLP"
              ],
              "ContainerReferenceNums": null,
              "ActualCrossing": {
                "LocalDateTimeOfArrival": "2025-04-28T19:00",
                "RouteId": "19"
              },
              "CheckedInCrossing": {
                "LocalDateTimeOfArrival": "2025-04-28T19:00",
                "RouteId": "19"
              },
              "PlannedCrossing": {
                "LocalDateTimeOfDeparture": "2025-04-28T19:00",
                "RouteId": "19"
              },
              "Declarations": {
                "Transits": [
                  {
                    "Id": "ABCD"
                  }
                ],
                "Customs": [
                  {
                    "Id": "ALVSCDSSTAND9930082"
                  }
                ]
              }
            }
            """;

        var deserializedGmr = JsonSerializer.Deserialize<Gmr>(gmr)!;
        await Verify(deserializedGmr).DontScrubDateTimes();
    }

    [Fact]
    public async Task Gmr_DeserializesCorrectly_PlannedCrossing_Departure_Nullable()
    {
        const string gmr = """
            {
              "GmrId": "GMRAADYA9J8G",
              "HaulierEori": "GB1196193155298",
              "State": "OPEN",
              "InspectionRequired": null,
              "ReportToLocations": null,
              "UpdatedDateTime": "2025-04-18T19:00:00.353Z",
              "Direction": "UK_INBOUND",
              "HaulierType": "NATO_MOD",
              "IsUnaccompanied": true,
              "VehicleRegNum": "RXPXOW",
              "TrailerRegistrationNums": [
                "7PIHPW",
                "E4FWLP"
              ],
              "ContainerReferenceNums": null,
              "ActualCrossing": {
                "LocalDateTimeOfArrival": "2025-04-28T19:00",
                "RouteId": "19"
              },
              "CheckedInCrossing": {
                "LocalDateTimeOfArrival": "2025-04-28T19:00",
                "RouteId": "19"
              },
              "PlannedCrossing": {
                "LocalDateTimeOfDeparture": null,
                "RouteId": "19"
              },
              "Declarations": {
                "Transits": [
                  {
                    "Id": "ABCD"
                  }
                ],
                "Customs": [
                  {
                    "Id": "ALVSCDSSTAND9930082"
                  }
                ]
              }
            }
            """;

        var deserializedGmr = JsonSerializer.Deserialize<Gmr>(gmr)!;
        await Verify(deserializedGmr).DontScrubDateTimes();
    }

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
            UpdatedDateTime = timestamp,
            Direction = "UK_INBOUND",
            HaulierType = "FPO_ASN",
            IsUnaccompanied = true,
            VehicleRegNum = "XCR5WN",
            TrailerRegistrationNums = ["W8YK34", "P33TNV"],
            ContainerReferenceNums = ["ABCD", "EFGH"],
            PlannedCrossing = new GmrPlannedCrossing { LocalDateTimeOfDeparture = timestamp, RouteId = "12345" },
            CheckedInCrossing = new GmrCheckedInCrossing { LocalDateTimeOfArrival = timestamp, RouteId = "12345" },
            ActualCrossing = new GmrActualCrossing { LocalDateTimeOfArrival = timestamp, RouteId = "12345" },
            Declarations = new GmrDeclarations
            {
                Customs = [new GmrDeclaration { Id = "12345" }],
                Transits = [new GmrDeclaration { Id = "12345" }],
            },
        };

        var dataApiGmr = (DataApiGvms.Gmr)gmr;

        await Verify(dataApiGmr).DontScrubDateTimes();
    }
}
