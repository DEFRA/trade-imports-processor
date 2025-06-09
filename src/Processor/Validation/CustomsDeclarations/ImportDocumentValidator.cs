using System.Collections.Frozen;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ImportDocumentValidator : AbstractValidator<ImportDocument>
{
    public ImportDocumentValidator(int itemNumber, string correlationId)
    {
        // CDMS-276
        RuleFor(p => p.DocumentCode)
            .Must(BeAValidDocumentCode!)
            .WithMessage(c =>
                $"DocumentCode {c.DocumentCode} on item number {itemNumber} is invalid. Your request with correlation ID {correlationId} has been terminated."
            )
            .WithState(p => "ALVSVAL308");

        RuleFor(p => p.DocumentStatus).NotEmpty().MaximumLength(2);

        RuleFor(p => p.DocumentControl).NotEmpty().MaximumLength(1);
    }

    private static readonly FrozenSet<string> s_documentCodes = CustomsDeclarationMappings
        .AuthorityDocumentChecks.Select(a => a.DocumentCode)
        .ToFrozenSet();

    private static bool BeAValidDocumentCode(string documentCode)
    {
        return !string.IsNullOrEmpty(documentCode) && s_documentCodes.Contains(documentCode);
    }
}
