using FluentValidation;
using Trade.Gateway.Api.Contract.Certificate;

namespace Defra.TradeImportsProcessor.Processor.Validation.TracesCheds;

public class TracesChedValidator : AbstractValidator<DefraUNVTDCHEDProfile>
{
    public TracesChedValidator()
    {
        RuleFor(g => g).Must(HaveLastUpdatedNote).WithMessage("No Last Updated note found");
    }

    private static bool HaveLastUpdatedNote(DefraUNVTDCHEDProfile ched)
    {
        return ched.LastUpdated.HasValue;
    }
}
