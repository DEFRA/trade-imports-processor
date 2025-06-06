using System.Text.Json.Serialization;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using DataApiErrors = Defra.TradeImportsDataApi.Domain.Errors;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundError
{
    [JsonPropertyName("header")]
    public required InboundErrorHeader Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("errors")]
    public required InboundErrorItem[] Errors { get; init; }

    public static explicit operator DataApiCustomsDeclaration.ExternalError(InboundError inboundError)
    {
        return new DataApiCustomsDeclaration.ExternalError
        {
            MessageSentAt = inboundError.ServiceHeader.ServiceCallTimestamp,
            SourceCorrelationId = inboundError.Header.SourceCorrelationId,
            ExternalCorrelationId = inboundError.ServiceHeader.CorrelationId,
            ExternalVersion = inboundError.Header.EntryVersionNumber,
            Errors = inboundError
                .Errors.Select(error => new DataApiErrors.ErrorItem
                {
                    Code = error.errorCode,
                    Message = error.errorMessage,
                })
                .ToArray(),
        };
    }
}
