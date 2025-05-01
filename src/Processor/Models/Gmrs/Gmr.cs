using Defra.TradeImportsDataApi.Domain.Gvms;
using DataApiGvms = Defra.TradeImportsDataApi.Domain.Gvms;

namespace Defra.TradeImportsProcessor.Processor.Models.Gmrs;

public class Gmr
{
    public string? GmrId { get; init; }
    public string? HaulierEori { get; init; }
    public string? State { get; init; }
    public bool? InspectionRequired { get; init; }
    public ReportToLocations[]? ReportToLocations { get; init; }
    public DateTime? UpdatedDateTime { get; init; }
    public string? Direction { get; init; }
    public string? HaulierType { get; init; }
    public bool? IsUnaccompanied { get; init; }
    public string? VehicleRegNum { get; init; }
    public string[]? TrailerRegistrationNums { get; init; }
    public string[]? ContainerReferenceNums { get; init; }
    public GmrPlannedCrossing? PlannedCrossing { get; init; }
    public GmrCheckedInCrossing? CheckedInCrossing { get; init; }
    public GmrActualCrossing? ActualCrossing { get; init; }
    public GmrDeclarations? Declarations { get; init; }

    public static explicit operator DataApiGvms.Gmr(Gmr gmr)
    {
        return new DataApiGvms.Gmr
        {
            Id = gmr.GmrId,
            HaulierEori = gmr.HaulierEori,
            State = DataApiGvms.State.Open, // TO-DO: fix when enums are removed in the API
            InspectionRequired = gmr.InspectionRequired,
            ReportToLocations = gmr.ReportToLocations,
            UpdatedSource = gmr.UpdatedDateTime,
            Direction = DataApiGvms.Direction.GbToNi, // TO-DO: fix when enums are removed in the API
            HaulierType = DataApiGvms.HaulierType.Etoe, // TO-DO: fix when enums are removed in the API
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
