using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public sealed record DocumentChecks
{
    public required string ExternalCorrelationId { get; init; }
    public required int ItemNumber { get; init; }
    public required ImportDocument Document { get; init; }
}
