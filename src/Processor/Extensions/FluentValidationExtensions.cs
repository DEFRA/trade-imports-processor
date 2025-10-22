using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithBtmsErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorCode,
        string? correlationId
    )
    {
        return rule.WithBtmsErrorCode(errorCode, _ => correlationId);
    }

    public static IRuleBuilderOptions<T, TProperty> WithBtmsErrorCode<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorCode,
        Func<T, string?> correlationIdFunc
    )
    {
        ArgumentNullException.ThrowIfNull(errorCode);
        ArgumentNullException.ThrowIfNull(correlationIdFunc);

        foreach (var componentRule in DefaultValidatorOptions.Configurable(rule).Components)
        {
            ((IRuleComponent<T, TProperty>)componentRule).SetErrorMessage(
                (ctx, val) =>
                {
                    var correlationId = correlationIdFunc(ctx.InstanceToValidate) ?? "UNKNOWN";
                    return $"There is an issue with the {{PropertyName}}. Your request with correlation Id {correlationId} has been terminated.";
                }
            );

            var wrapper = new Func<ValidationContext<T>, TProperty, object>((ctx, _) => errorCode);
            ((IRuleComponent<T, TProperty>)componentRule).CustomStateProvider = wrapper;
        }

        return rule;
    }

    public static IRuleBuilderOptions<T, decimal?> HaveAValidDecimalFormat<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder
    )
    {
        return ruleBuilder.Must(value =>
        {
            if (value == null)
                return true;

            var valueString = value.ToString() ?? "";
            var length = valueString.Replace(".", "").Length;
            var numDecimals = valueString.SkipWhile(c => c != '.').Skip(1).Count();
            return length <= 14 && numDecimals <= 3;
        });
    }
}
