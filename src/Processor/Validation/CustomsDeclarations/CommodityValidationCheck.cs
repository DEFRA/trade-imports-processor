using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public class CommodityValidationCheck
{
    public required string ExternalCorrelationId { get; init; }
    public required Commodity Commodity { get; init; }
}
