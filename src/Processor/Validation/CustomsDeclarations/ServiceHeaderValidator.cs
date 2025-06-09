using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ServiceHeaderValidator : AbstractValidator<ServiceHeader>
{
    public ServiceHeaderValidator()
    {
        RuleFor(p => p.CorrelationId).NotEmpty().MaximumLength(20);

        // CDMS-252
        RuleFor(p => p.SourceSystem)
            .Must(p => p == "CDS")
            .WithMessage(c =>
                $"Source system {c.SourceSystem} is invalid. Your request with correlation ID {c.CorrelationId} has been terminated."
            )
            .WithState(p => "ALVSVAL101");

        // CDMS-253
        RuleFor(p => p.DestinationSystem)
            .Must(p => p == "ALVS")
            .WithMessage(c =>
                $"Destination system {c.DestinationSystem} is invalid. Your request with correlation ID {c.CorrelationId} has been terminated."
            )
            .WithState(p => "ALVSVAL102");
    }
}
