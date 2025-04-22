using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ClearanceRequestValidator : AbstractValidator<ClearanceRequestValidatorInput>
{
    public ClearanceRequestValidator()
    {
        When(
            p => p.ExistingClearanceRequest is not null,
            () =>
            {
                RuleFor(p => p)
                    .Must(p => NotBeADuplicateEntryVersionNumber(p.NewClearanceRequest, p.ExistingClearanceRequest))
                    .WithState(p => "ALVSVAL303")
                    .WithMessage(p =>
                        $"The import declaration was received as a new declaration. There is already a current import declaration in BTMS with EntryReference {p.Mrn} and EntryVersionNumber {p.ExistingClearanceRequest?.ExternalVersion}. Your request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );

                RuleFor(p => p.ExistingFinalisation)
                    .Must(NotBeCancelled)
                    .WithState(p => "ALVSVAL324")
                    .WithMessage(p =>
                        $"The Import Declaration with Entry Reference {p.Mrn} and EntryVersionNumber {p.NewClearanceRequest.ExternalVersion} was received but the Import Declaration was cancelled. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );
    }

    private static bool NotBeADuplicateEntryVersionNumber(
        ClearanceRequest newClearanceRequest,
        ClearanceRequest? existingClearanceRequest
    )
    {
        return newClearanceRequest.ExternalVersion != existingClearanceRequest?.ExternalVersion;
    }

    private static bool NotBeCancelled(Finalisation? existingFinalisation)
    {
        return !existingFinalisation?.FinalState.IsCancelled() ?? true;
    }
}
