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
    public DateTime? UpdatedSource { get; init; }

    [JsonPropertyName("direction")]
    public string? Direction { get; init; }

    [JsonPropertyName("haulierType")]
    public string? HaulierType { get; init; }

    [JsonPropertyName("isUnaccompanied")]
    public bool? IsUnaccompanied { get; init; }

    [JsonPropertyName("vehicleRegNum")]
    public string? VehicleRegistrationNumber { get; init; }

    [JsonPropertyName("trailerRegistrationNums")]
    public string[]? TrailerRegistrationNums { get; init; }

    [JsonPropertyName("containerReferenceNums")]
    public string[]? ContainerReferenceNums { get; init; }

    [JsonPropertyName("plannedCrossing")]
    public PlannedCrossing? PlannedCrossing { get; init; }

    [JsonPropertyName("checkedInCrossing")]
    public CheckedInCrossing? CheckedInCrossing { get; init; }

    [JsonPropertyName("actualCrossing")]
    public ActualCrossing? ActualCrossing { get; init; }

    [JsonPropertyName("declarations")]
    public Declarations? Declarations { get; init; }

    public static explicit operator DataApiGvms.Gmr(Gmr gmr)
    {
        return new DataApiGvms.Gmr
        {
            Id = gmr.GmrId,
            HaulierEori = gmr.HaulierEori,
            State = DataApiGvms.State.Open, // TO-DO: fix when enums are removed in the API
            InspectionRequired = gmr.InspectionRequired,
            ReportToLocations = gmr.ReportToLocations,
            UpdatedSource = gmr.UpdatedSource,
            Direction = DataApiGvms.Direction.GbToNi, // TO-DO: fix when enums are removed in the API
            HaulierType = DataApiGvms.HaulierType.Etoe, // TO-DO: fix when enums are removed in the API
            IsUnaccompanied = gmr.IsUnaccompanied,
            VehicleRegistrationNumber = gmr.VehicleRegistrationNumber,
            TrailerRegistrationNums = gmr.TrailerRegistrationNums,
            ContainerReferenceNums = gmr.ContainerReferenceNums,
            PlannedCrossing = gmr.PlannedCrossing,
            CheckedInCrossing = gmr.CheckedInCrossing,
            ActualCrossing = gmr.ActualCrossing,
            Declarations = gmr.Declarations,
        };
    }
}
