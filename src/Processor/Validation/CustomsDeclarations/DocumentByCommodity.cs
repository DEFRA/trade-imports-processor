using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record DocumentByCommodity
{
    public required Commodity Commodity { get; init; }
    public required ImportDocument Document { get; init; }
}
