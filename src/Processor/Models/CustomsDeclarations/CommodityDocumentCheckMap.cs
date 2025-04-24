namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public record CommodityDocumentCheckMap
{
    public required string CheckCode { get; init; }
    public required string DocumentCode { get; init; }
}
