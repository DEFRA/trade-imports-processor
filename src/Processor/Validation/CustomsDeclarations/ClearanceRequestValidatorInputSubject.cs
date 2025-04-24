namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public record ClearanceRequestValidatorInputSubject<T> : ClearanceRequestValidatorInput
{
    public required T Subject { get; init; }
}
