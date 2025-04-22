using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record ClearanceRequestValidatorInput
{
    public required string Mrn { get; init; }
    public required ClearanceRequest NewClearanceRequest { get; init; }
    public ClearanceRequest? ExistingClearanceRequest { get; init; }
    public Finalisation? ExistingFinalisation { get; init; }
}
