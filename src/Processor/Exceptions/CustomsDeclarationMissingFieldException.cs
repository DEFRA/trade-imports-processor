using System.Runtime.CompilerServices;

namespace Defra.TradeImportsProcessor.Processor.Exceptions;

public class CustomsDeclarationMessageMissingFieldException<T>(
    object? value,
    [CallerArgumentExpression(nameof(value))] string? expression = null
) : Exception(GetMessage(value, expression))
{
    private static string GetMessage(object? value, string? expression)
    {
        _ = value;
        return $"Invalid customs declaration - field '{GetFieldName(expression)}' in type '{nameof(T)}' is required";
    }

    private static string GetFieldName(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return string.Empty;
        }

        var firstDotIndex = expression.IndexOf('.');
        return firstDotIndex >= 0 ? expression[(firstDotIndex + 1)..] : expression;
    }
}
