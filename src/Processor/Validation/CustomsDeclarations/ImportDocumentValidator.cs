using System.Collections.Frozen;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Validation;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ImportDocumentValidator : AbstractValidator<ImportDocument>
{
    public ImportDocumentValidator(int itemNumber, string correlationId)
    {
        RuleFor(p => p.DocumentReference)
            .Must(reference => reference?.Value.Length <= 35)
            .When(p => p.DocumentReference != null)
            .WithBtmsErrorCode("ERR025", correlationId);

        RuleFor(p => p.DocumentControl).NotEmpty().MaximumLength(1).WithBtmsErrorCode("ERR026", correlationId);

        RuleFor(p => p.DocumentStatus).NotEmpty().MaximumLength(2).WithBtmsErrorCode("ERR027", correlationId);

        RuleFor(p => p.DocumentQuantity)
            .HaveAValidDecimalFormat()
            .When(p => p.DocumentQuantity != null)
            .WithBtmsErrorCode("ERR028", correlationId);

        // CDMS-276
        RuleFor(p => p.DocumentCode)
            .Must(BeAValidDocumentCode!)
            .WithMessage(c =>
                $"DocumentCode {c.DocumentCode} on item number {itemNumber} is invalid. Your request with correlation ID {correlationId} has been terminated."
            )
            .WithState(p => "ALVSVAL308");
    }

    private static readonly FrozenSet<string> s_documentCodes = CustomsDeclarationMappings
        .AuthorityDocumentChecks.Select(a => a.DocumentCode)
        .ToFrozenSet();

    private static bool BeAValidDocumentCode(string documentCode)
    {
        return !string.IsNullOrEmpty(documentCode) && s_documentCodes.Contains(documentCode);
    }
}
