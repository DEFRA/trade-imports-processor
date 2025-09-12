using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class HeaderValidator : AbstractValidator<Header>
{
    public HeaderValidator(string correlationId)
    {
        RuleFor(p => p.EntryReference)
            .Matches("^[1-9]{2}[A-Za-z]{2}[A-Za-z0-9]{14}$")
            .WithBtmsErrorCode("ERR003", correlationId);

        RuleFor(p => p.EntryVersionNumber).InclusiveBetween(1, 99).WithBtmsErrorCode("ERR004", correlationId);

        // CDMS-256
        RuleFor(p => p.EntryVersionNumber)
            .NotEmpty()
            .WithState(p => "ALVSVAL153")
            .WithMessage(p =>
                $"EntryVersionNumber has not been provided for the import document. Provide an EntryVersionNumber. Your request with correlation ID {correlationId} has been terminated."
            );
    }
}
