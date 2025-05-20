using System.Collections.Frozen;
using Defra.TradeImportsDataApi.Domain.Errors;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class ErrorNotificationValidator : AbstractValidator<ErrorNotification>
{
    private static readonly FrozenSet<string> s_validInboundErrorCodes = new[]
    {
        "HMRCVAL101",
        "HMRCVAL102",
        "HMRCVAL103",
        "HMRCVAL104",
    }.ToFrozenSet();

    public ErrorNotificationValidator()
    {
        // ???
        RuleForEach(n => n.Errors)
            .Must(BeAValidErrorCode)
            .WithMessage(
                (n, e) => $"The error code {e.Code} is not valid for correlation ID {n.ExternalCorrelationId}"
            );
    }

    private static bool BeAValidErrorCode(ErrorNotification notification, ErrorItem item)
    {
        return s_validInboundErrorCodes.Contains(item.Code);
    }
}
