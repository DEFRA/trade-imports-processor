using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record CheckByCommodity
{
    public required CommodityCheck Check { get; init; }
    public required Commodity Commodity { get; init; }
}
