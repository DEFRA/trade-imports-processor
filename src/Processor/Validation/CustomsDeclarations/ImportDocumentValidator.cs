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

    private static readonly List<string> s_documentCodes =
    [
        "C085",
        "C633",
        "C640",
        "C641",
        "C673",
        "N002",
        "N851",
        "N852",
        "C678",
        "N853",
        "9HCG",
        "9115",
    ];

    private static bool BeAValidDocumentCode(string documentCode)
    {
        return !string.IsNullOrEmpty(documentCode) && s_documentCodes.Contains(documentCode);
    }
}
