using BtmsGateway.Contract.CustomsDeclarations;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Extensions.Model
{
    public static class FinalisationExtensions
    {
        public static DataApiCustomsDeclaration.Finalisation ToDataApiFinalisation(this Finalisation finalisation)
        {
            if (finalisation.ServiceHeader.ServiceCallTimestamp == null)
            {
                throw new InvalidOperationException();
            }

            if (finalisation.Header.EntryVersionNumber == null)
            {
                throw new InvalidOperationException();
            }

            return new DataApiCustomsDeclaration.Finalisation
            {
                ExternalCorrelationId = finalisation.ServiceHeader.CorrelationId,
                MessageSentAt = finalisation.ServiceHeader.ServiceCallTimestamp.Value,
                ExternalVersion = finalisation.Header.EntryVersionNumber.Value,
                DecisionNumber = finalisation.Header.DecisionNumber,
                FinalState = finalisation.Header.FinalState,
                IsManualRelease = finalisation.Header.ManualAction == "Y",
            };
        }
    }
}
