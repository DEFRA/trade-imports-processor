using Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record HeaderValidatorInput
{
    public required Header Header { get; init; }
    public required ServiceHeader ServiceHeader { get; init; }
};
