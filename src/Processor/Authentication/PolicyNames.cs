using System.Diagnostics.CodeAnalysis;

namespace Defra.TradeImportsProcessor.Processor.Authentication;

[ExcludeFromCodeCoverage(Justification = "This is covered by integration tests")]
public static class PolicyNames
{
    public const string Execute = nameof(Scopes.Execute);
}
