using System.Text.Json.Serialization;
using Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;
using FluentValidation.Results;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class CustomsDeclarationsBase
{
    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }
}
