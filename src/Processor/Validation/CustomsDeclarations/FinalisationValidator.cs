using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class FinalisationValidator : AbstractValidator<FinalisationValidatorInput>
{
    public FinalisationValidator()
    {
        RuleFor(p => p.NewFinalisation)
            .Must(
                (p, f) =>
                    f.FinalState != FinalState.CancelledAfterArrival
                    && f.FinalState != FinalState.CancelledWhilePreLodged
                    && f.ExternalVersion == p.ExistingClearanceRequest.ExternalVersion
            )
            .WithState(_ => "ALVSVAL401")
            .WithMessage(
                (p, f) =>
                    $"The finalised state was received for EntryReference {p.Mrn} EntryVersionNumber {f.ExternalVersion}. This has already been replaced by a later version of the import declaration. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation)
            .Must((p, f) => Enum.IsDefined(f.FinalState))
            .WithState(_ => "ALVSVAL402")
            .WithMessage(
                (p, f) =>
                    $"The FinalState {f.FinalState} is invalid. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation)
            .Must(
                (p, f) =>
                    p.ExistingFinalisation?.FinalState != FinalState.CancelledAfterArrival
                    && p.ExistingFinalisation?.FinalState != FinalState.CancelledWhilePreLodged
            )
            .WithState(_ => "ALVSVAL403")
            .WithMessage(
                (p, f) =>
                    $"The final state was received for EntryReference {p.Mrn} EntryVersionNumber {f.ExternalVersion} but the import declaration was cancelled. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation)
            .Must(
                (p, f) =>
                    p.ExistingFinalisation?.FinalState != FinalState.CancelledAfterArrival
                    && p.ExistingFinalisation?.FinalState != FinalState.CancelledWhilePreLodged
                    && f.FinalState != FinalState.CancelledAfterArrival
                    && f.FinalState != FinalState.CancelledWhilePreLodged
            )
            .WithState(_ => "ALVSVAL404")
            .WithMessage(
                (p, f) =>
                    $"An attempt to cancel EntryReference {p.Mrn} EntryVersionNumber {f.ExternalVersion} was made but the import declaration was cancelled. Your request with correlation ID {f.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation)
            .Must(
                (p, f) =>
                    f.FinalState != FinalState.CancelledAfterArrival
                    && f.FinalState != FinalState.CancelledWhilePreLodged
                    && p.ExistingClearanceRequest.ExternalVersion == f.ExternalVersion
            )
            .WithState(_ => "ALVSVAL405")
            .WithMessage(
                (p, f) =>
                    $"The import declaration was received as a cancellation. The EntryReference {p.Mrn} EntryVersionNumber {f.ExternalVersion} have already been replaced by a later version. Your request with correlation ID {f.ExternalCorrelationId} has been terminated."
            );
    }
}
