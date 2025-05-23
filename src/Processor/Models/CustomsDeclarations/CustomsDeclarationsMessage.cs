using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class CustomsDeclarationsMessage : CustomsDeclarationsBase
{
    [JsonPropertyName("header")]
    public required Header Header { get; init; }
}
