using BtmsGateway.Contract.CustomsDeclarations;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class CustomsDeclarationsMessageValidator : AbstractValidator<CustomsDeclarationsMessage>
{
    public CustomsDeclarationsMessageValidator()
    {
        RuleFor(p => p.ServiceHeader.CorrelationId).NotNull();
        RuleFor(p => p.ServiceHeader).SetValidator(new ServiceHeaderValidator());
        RuleFor(p => p.Header).SetValidator(p => new HeaderValidator(p.ServiceHeader.CorrelationId!));
    }
}
