using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class HeaderValidator : AbstractValidator<Header>
{
    public HeaderValidator(string correlationId)
    {
        RuleFor(p => p.EntryReference)
            .NotEmpty()
            .MaximumLength(22)
            .Matches("[1-9]{2}[A-Za-z]{2}[A-Za-z0-9]{14}")
            .WithState(p => "ALVSVAL303");

        RuleFor(p => p.EntryVersionNumber)
            .NotNull()
            .WithState(p => "ALVSVAL153")
            .WithMessage(p =>
                $"EntryVersionNumber has not been provided for the import document. Provide an EntryVersionNumber. Your request with correlation ID {correlationId} has been terminated."
            );

        RuleFor(p => p.EntryVersionNumber).InclusiveBetween(1, 99);
    }
}
