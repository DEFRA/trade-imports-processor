using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class FinalisationValidator : AbstractValidator<FinalisationValidatorInput>
{
    public FinalisationValidator()
    {
        RuleFor(p => p.NewFinalisation.ExternalVersion).NotNull().InclusiveBetween(1, 99);

        RuleFor(p => p.Mrn)
            .NotEmpty()
            .MaximumLength(22)
            .Matches("[1-9]{2}[A-Za-z]{2}[A-Za-z0-9]{14}")
            .WithState(_ => "ALVSVAL401");

        RuleFor(p => p.NewFinalisation.FinalState)
            .Must(BeANewFinalisation)
            .WithState(_ => "ALVSVAL401")
            .WithMessage(p =>
                $"The finalised state was received for EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion}. This has already been replaced by a later version of the import declaration. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation)
            .Must((_, f) => Enum.IsDefined(f.FinalState))
            .WithState(_ => "ALVSVAL402")
            .WithMessage(
                (p, f) =>
                    $"The FinalState {f.FinalState} is invalid. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation.FinalState)
            .Must(NotBeAlreadyCancelled)
            .WithState(_ => "ALVSVAL403")
            .WithMessage(p =>
                $"The final state was received for EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} but the import declaration was cancelled. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation.FinalState)
            .Must(NotBeACancellationWhenAlreadyCancelled)
            .WithState(_ => "ALVSVAL501")
            .WithMessage(p =>
                $"An attempt to cancel EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} was made but the import declaration was cancelled. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        RuleFor(p => p.NewFinalisation.FinalState)
            .Must(BeAValidCancellationRequest)
            .WithState(_ => "ALVSVAL506")
            .WithMessage(p =>
                $"The import declaration was received as a cancellation. The EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} have already been replaced by a later version. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );
    }

    private static bool BeANewFinalisation(FinalisationValidatorInput p, FinalState finalState)
    {
        return finalState is not (FinalState.CancelledAfterArrival or FinalState.CancelledWhilePreLodged)
            && p.NewFinalisation.ExternalVersion == p.ExistingClearanceRequest.ExternalVersion;
    }

    private static bool NotBeAlreadyCancelled(FinalisationValidatorInput p, FinalState finalState)
    {
        return p.ExistingFinalisation?.FinalState != FinalState.CancelledAfterArrival
            && p.ExistingFinalisation?.FinalState != FinalState.CancelledWhilePreLodged;
    }

    private static bool NotBeACancellationWhenAlreadyCancelled(FinalisationValidatorInput p, FinalState finalState)
    {
        var alreadyCancelled =
            p.ExistingFinalisation?.FinalState
                is FinalState.CancelledAfterArrival
                    or FinalState.CancelledWhilePreLodged;
        var willCancel = finalState is FinalState.CancelledAfterArrival or FinalState.CancelledWhilePreLodged;

        return !alreadyCancelled && !willCancel;
    }

    private static bool BeAValidCancellationRequest(FinalisationValidatorInput p, FinalState finalState)
    {
        var isCancellation = finalState is FinalState.CancelledAfterArrival or FinalState.CancelledWhilePreLodged;
        return !isCancellation || p.ExistingClearanceRequest.ExternalVersion == p.NewFinalisation.ExternalVersion;
    }
}
