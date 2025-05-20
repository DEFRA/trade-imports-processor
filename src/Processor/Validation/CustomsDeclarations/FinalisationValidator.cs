using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class FinalisationValidator : AbstractValidator<FinalisationValidatorInput>
{
    public FinalisationValidator()
    {
        RuleFor(p => p.NewFinalisation.ExternalVersion).NotNull().InclusiveBetween(1, 99);

        // INCORRECT ERROR CODE
        RuleFor(p => p.Mrn)
            .NotEmpty()
            .MaximumLength(22)
            .Matches("[1-9]{2}[A-Za-z]{2}[A-Za-z0-9]{14}")
            .WithState(_ => "ALVSVAL401");

        // CDMS-269 - NEW
        RuleFor(p => p.NewFinalisation)
            .Must((_, f) => Enum.IsDefined(f.FinalStateValue()))
            .WithState(_ => "ALVSVAL402")
            .WithMessage(
                (p, f) =>
                    $"The FinalState {f.FinalState} is invalid. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
            );

        When(
            p => p.ExistingFinalisation is not null,
            () =>
            {
                // CDMS-268
                //Disabled as part of https://eaflood.atlassian.net/browse/CDMS-685 until we better understand this rule
                ////RuleFor(p => p.NewFinalisation.FinalStateValue())
                ////    .Must(BeANewFinalisation)
                ////    .WithState(_ => "ALVSVAL401")
                ////    .WithMessage(p =>
                ////        $"The finalised state was received for EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion}. This has already been replaced by a later version of the import declaration. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
                ////    );

                // CDMS-270
                RuleFor(p => p.ExistingFinalisation!)
                    .Must(NotBeAlreadyCancelled)
                    .WithState(_ => "ALVSVAL403")
                    .WithMessage(p =>
                        $"The final state was received for EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} but the import declaration was cancelled. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
                    );

                // CDMS-271
                RuleFor(p => p.NewFinalisation.FinalStateValue())
                    .Must(NotBeACancellationWhenAlreadyCancelled)
                    .WithState(_ => "ALVSVAL501")
                    .WithMessage(p =>
                        $"An attempt to cancel EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} was made but the import declaration was cancelled. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
                    );

                // CDMS-272
                RuleFor(p => p.NewFinalisation.FinalStateValue())
                    .Must(BeAValidCancellationRequest)
                    .WithState(_ => "ALVSVAL506")
                    .WithMessage(p =>
                        $"The import declaration was received as a cancellation. The EntryReference {p.Mrn} EntryVersionNumber {p.NewFinalisation.ExternalVersion} have already been replaced by a later version. Your request with correlation ID {p.NewFinalisation.ExternalCorrelationId} has been terminated."
                    );
            }
        );
    }

    //Disabled as part of https://eaflood.atlassian.net/browse/CDMS-685 until we better understand this rule
    ////private static bool BeANewFinalisation(FinalisationValidatorInput p, FinalStateValues newFinalState)
    ////{
    ////    return newFinalState.IsNotCancelled()
    ////        && p.NewFinalisation.ExternalVersion == p.ExistingClearanceRequest.ExternalVersion;
    ////}

    private static bool NotBeAlreadyCancelled(FinalisationValidatorInput p, Finalisation existingFinalisation)
    {
        return existingFinalisation.FinalStateValue().IsNotCancelled();
    }

    private static bool NotBeACancellationWhenAlreadyCancelled(
        FinalisationValidatorInput p,
        FinalStateValues newFinalState
    )
    {
        if (p.ExistingFinalisation == null)
            return true;

        return !(newFinalState.IsCancelled() && p.ExistingFinalisation.FinalStateValue().IsCancelled());
    }

    private static bool BeAValidCancellationRequest(FinalisationValidatorInput p, FinalStateValues newFinalState)
    {
        return newFinalState.IsCancelled()
            && p.ExistingClearanceRequest.ExternalVersion == p.NewFinalisation.ExternalVersion;
    }
}
