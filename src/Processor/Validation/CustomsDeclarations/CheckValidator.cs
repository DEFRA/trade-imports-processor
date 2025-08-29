using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using Defra.TradeImportsProcessor.Processor.Extensions;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class CheckValidator : AbstractValidator<CommodityCheck>
{
    public CheckValidator(int itemNumber, string correlationId)
    {
        RuleFor(p => p.CheckCode).MaximumLength(4).WithBtmsErrorCode("ERR029", correlationId);
        RuleFor(p => p.DepartmentCode).NotEmpty().MaximumLength(8).WithBtmsErrorCode("ERR030", correlationId);

        // CDMS-258
        RuleFor(p => p.CheckCode)
            .NotEmpty()
            .WithMessage(_ =>
                $"The CheckCode field on item number {itemNumber} must have a value. Your service request with Correlation ID {correlationId} has been terminated."
            )
            .WithState(_ => "ALVSVAL311");
    }
}
