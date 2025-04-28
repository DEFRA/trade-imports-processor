using System.Text.Json.Serialization;
using DataApiErrors = Defra.TradeImportsDataApi.Domain.Errors;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class InboundError
{
    [JsonPropertyName("header")]
    public required Header Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("errors")]
    public required InboundErrorItem[] Errors { get; init; }

    public static explicit operator DataApiErrors.ErrorNotification(InboundError inboundError)
    {
        return new DataApiErrors.ErrorNotification
        {
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
