using System.Text.Json.Serialization;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundError
{
    [JsonPropertyName("header")]
    public required Header Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("errors")]
    public required InboundErrorItem[] Errors { get; init; }

    public static explicit operator DataApiCustomsDeclaration.InboundErrorNotification(InboundError inboundError)
    {
        return new DataApiCustomsDeclaration.InboundErrorNotification
        {
            ExternalCorrelationId = inboundError.ServiceHeader.CorrelationId,
            ExternalVersion = inboundError.Header.EntryVersionNumber,
            Errors = inboundError
                .Errors.Select(error => new DataApiCustomsDeclaration.InboundErrorItem
                {
                    Code = error.errorCode,
                    Message = error.errorMessage,
                })
                .ToArray(),
        };
    }
}
