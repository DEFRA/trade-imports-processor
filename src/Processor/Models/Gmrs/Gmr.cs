using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.Gvms;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class Gmr
{
    [JsonPropertyName("gmrId")]
    public string? GmrId { get; init; }

    [JsonPropertyName("haulierEORI")]
    public string? HaulierEori { get; init; }

    [JsonPropertyName("state")]
    public string? State { get; init; }

    [JsonPropertyName("inspectionRequired")]
    public bool? InspectionRequired { get; init; }

    [JsonPropertyName("reportToLocations")]
    public ReportToLocations[]? ReportToLocations { get; init; }

    [JsonPropertyName("updatedDateTime")]
    public DateTime? UpdatedDateTime { get; init; }

    [JsonPropertyName("direction")]
    public string? Direction { get; init; }

    [JsonPropertyName("haulierType")]
    public string? HaulierType { get; init; }

    [JsonPropertyName("isUnaccompanied")]
    public bool? IsUnaccompanied { get; init; }

    [JsonPropertyName("vehicleRegNum")]
    public string? VehicleRegNum { get; init; }

    [JsonPropertyName("trailerRegistrationNums")]
    public string[]? TrailerRegistrationNums { get; init; }

    [JsonPropertyName("containerReferenceNums")]
    public string[]? ContainerReferenceNums { get; init; }

    [JsonPropertyName("plannedCrossing")]
    public GmrPlannedCrossing? PlannedCrossing { get; init; }

    [JsonPropertyName("checkedInCrossing")]
    public GmrCheckedInCrossing? CheckedInCrossing { get; init; }

    [JsonPropertyName("actualCrossing")]
    public GmrActualCrossing? ActualCrossing { get; init; }

    [JsonPropertyName("declarations")]
    public GmrDeclarations? Declarations { get; init; }

    public static explicit operator DataApiGvms.Gmr(Gmr gmr)
    {
        return new DataApiGvms.Gmr
        {
            Id = gmr.GmrId,
            HaulierEori = gmr.HaulierEori,
            State = gmr.State,
            InspectionRequired = gmr.InspectionRequired,
            ReportToLocations = gmr.ReportToLocations,
            UpdatedSource = gmr.UpdatedDateTime,
            Direction = gmr.Direction,
            HaulierType = gmr.HaulierType,
            IsUnaccompanied = gmr.IsUnaccompanied,
            VehicleRegistrationNumber = gmr.VehicleRegNum,
            TrailerRegistrationNums = gmr.TrailerRegistrationNums,
            ContainerReferenceNums = gmr.ContainerReferenceNums,
            PlannedCrossing =
                gmr.PlannedCrossing != null
                    ? new DataApiGvms.PlannedCrossing
                    {
                        DepartsAt = gmr.PlannedCrossing?.LocalDateTimeOfDeparture,
                        RouteId = gmr.PlannedCrossing?.RouteId,
                    }
                    : null,
            CheckedInCrossing =
                gmr.CheckedInCrossing != null
                    ? new DataApiGvms.CheckedInCrossing
                    {
                        ArrivesAt = gmr.CheckedInCrossing?.LocalDateTimeOfArrival,
                        RouteId = gmr.CheckedInCrossing?.RouteId,
                    }
                    : null,
            ActualCrossing =
                gmr.ActualCrossing != null
                    ? new DataApiGvms.ActualCrossing
                    {
                        ArrivesAt = gmr.ActualCrossing?.LocalDateTimeOfArrival,
                        RouteId = gmr.ActualCrossing?.RouteId,
                    }
                    : null,
            Declarations =
                gmr.Declarations != null
                    ? new DataApiGvms.Declarations
                    {
                        Customs = gmr.Declarations.Customs?.Select(d => new Customs { Id = d.Id }).ToArray(),
                        Transits = gmr.Declarations.Transits?.Select(d => new Transits { Id = d.Id }).ToArray(),
                    }
                    : null,
        };
    }
}
