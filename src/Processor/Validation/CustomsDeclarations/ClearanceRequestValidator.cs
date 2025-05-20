using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Finalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ClearanceRequestValidator : AbstractValidator<ClearanceRequestValidatorInput>
{
    public ClearanceRequestValidator()
    {
        RuleFor(p => p.NewClearanceRequest.DeclarationUcr).MaximumLength(35);
        RuleFor(p => p.NewClearanceRequest.DeclarantId).NotEmpty().MaximumLength(18);
        RuleFor(p => p.NewClearanceRequest.DeclarantName).NotEmpty().MaximumLength(35);
        RuleFor(p => p.NewClearanceRequest.DispatchCountryCode).NotEmpty().Length(2);
        RuleFor(p => p.NewClearanceRequest.DeclarationType).Must(p => p is "S" or "F");
        RuleForEach(p => p.NewClearanceRequest.Commodities)
            .SetValidator(p => new CommodityValidator(p.NewClearanceRequest.ExternalCorrelationId!));
        RuleFor(p => p.NewClearanceRequest.ExternalVersion).InclusiveBetween(1, 99);

        // CDMS-255
        RuleFor(p => p.NewClearanceRequest.PreviousExternalVersion)
            .NotNull()
            .WithState(_ => "ALVSVAL152")
            .WithMessage(p =>
                $"PreviousVersionNumber has not been provided for the import document. Provide a PreviousVersionNumber. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            )
            .When(p => p.NewClearanceRequest.ExternalVersion > 1);

        // CDMS-256
        RuleFor(p => p.NewClearanceRequest.ExternalVersion)
            .NotNull()
            .WithState(_ => "ALVSVAL153")
            .WithMessage(p =>
                $"EntryVersionNumber has not been provided for the import document. Provide an EntryVersionNumber. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        // CDMS-257 - NEW
        RuleForEach(p =>
                (p.NewClearanceRequest.Commodities ?? Array.Empty<Commodity>()).GroupBy(c => c.ItemNumber).ToList()
            )
            .Must(grouping => grouping.Count() == 1)
            .OverridePropertyName("Commodities")
            .WithState(_ => "ALVSVAL164")
            .WithMessage(
                (p, grouping) =>
                    $"Item number {grouping.Key} is invalid as it appears more than once. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        // CDMS-378
        When(
            p => p.ExistingClearanceRequest is not null,
            () =>
            {
                RuleFor(p => p)
                    .Must(p => NotBeADuplicateEntryVersionNumber(p.NewClearanceRequest, p.ExistingClearanceRequest))
                    .WithState(_ => "ALVSVAL303")
                    .WithMessage(p =>
                        $"The import declaration was received as a new declaration. There is already a current import declaration in BTMS with EntryReference {p.Mrn} and EntryVersionNumber {p.ExistingClearanceRequest?.ExternalVersion}. Your request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );

        // CMDS-259
        RuleFor(p => p.NewClearanceRequest.DeclarationUcr)
            .NotEmpty()
            .WithState(_ => "ALVSVAL313")
            .WithMessage(p =>
                $"The DeclarationUCR field must have a value.  Your service request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            );

        // CDMS-266
        When(
            p => p.ExistingFinalisation is not null,
            () =>
            {
                RuleFor(p => p.ExistingFinalisation)
                    .Must(NotBeCancelled)
                    .WithState(_ => "ALVSVAL324")
                    .WithMessage(p =>
                        $"The Import Declaration with Entry Reference {p.Mrn} and EntryVersionNumber {p.NewClearanceRequest.ExternalVersion} was received but the Import Declaration was cancelled. Your request with correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
                    );
            }
        );

        // CDMS-333
        RuleFor(p => p)
            .Must(p => p.NewClearanceRequest.ExternalVersion > p.NewClearanceRequest.PreviousExternalVersion)
            .WithState(_ => "ALVSVAL326")
            .WithMessage(p =>
                $"The previous version number {p.NewClearanceRequest.PreviousExternalVersion} on the entry document must be less than the entry version number. Your service request with Correlation ID {p.NewClearanceRequest.ExternalCorrelationId} has been terminated."
            )
            .When(p => p.NewClearanceRequest.PreviousExternalVersion.HasValue);
    }

    private static bool NotBeADuplicateEntryVersionNumber(
        DataApiCustomsDeclaration.ClearanceRequest newClearanceRequest,
        DataApiCustomsDeclaration.ClearanceRequest? existingClearanceRequest
    )
    {
        return newClearanceRequest.ExternalVersion != existingClearanceRequest?.ExternalVersion;
    }

    private static bool NotBeCancelled(Finalisation? existingFinalisation)
    {
        return !(existingFinalisation?.FinalStateValue().IsCancelled()).GetValueOrDefault();
    }
}
