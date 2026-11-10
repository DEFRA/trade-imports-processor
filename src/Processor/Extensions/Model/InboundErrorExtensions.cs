using BtmsGateway.Contract.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using DataApiErrors = Defra.TradeImportsDataApi.Domain.Errors;

namespace Defra.TradeImportsProcessor.Processor.Extensions.Model
{
    public static class InboundErrorExtensions
    {
        public static DataApiCustomsDeclaration.ExternalError ToDataApiExternalError(this InboundError inboundError)
        {
            if (inboundError.ServiceHeader.ServiceCallTimestamp == null)
            {
                throw new InvalidOperationException();
            }

            return new DataApiCustomsDeclaration.ExternalError
            {
                MessageSentAt = inboundError.ServiceHeader.ServiceCallTimestamp.Value,
                SourceCorrelationId = inboundError.Header.SourceCorrelationId,
                ExternalCorrelationId = inboundError.ServiceHeader.CorrelationId,
                ExternalVersion = inboundError.Header.EntryVersionNumber,
                Errors = inboundError
                    .Errors.Select(error => new DataApiErrors.ErrorItem
                    {
                        Code = error.ErrorCode,
                        Message = error.ErrorMessage,
                    })
                    .ToArray(),
            };
        }
    }
}
