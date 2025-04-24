using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record FinalisationValidatorInput
{
    public Finalisation? ExistingFinalisation { get; init; }
    public required ClearanceRequest ExistingClearanceRequest { get; init; }
    public required string Mrn { get; init; }
    public required Finalisation NewFinalisation { get; init; }
}
