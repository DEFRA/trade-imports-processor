using System.Text.Json.Serialization;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class Finalisation
{
    [JsonPropertyName("header")]
    public required FinalisationHeader Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    public static explicit operator DataApiCustomsDeclaration.Finalisation(Finalisation finalisation)
    {
        return new DataApiCustomsDeclaration.Finalisation
        {
            ExternalCorrelationId = finalisation.ServiceHeader.CorrelationId,
            MessageSentAt = finalisation.ServiceHeader.ServiceCallTimestamp!,
            ExternalVersion = finalisation.Header.EntryVersionNumber,
            DecisionNumber = finalisation.Header.DecisionNumber,
            FinalState = (DataApiCustomsDeclaration.FinalState)int.Parse(finalisation.Header.FinalState),
            IsManualRelease = finalisation.Header.ManualAction == "Y",
        };
    }
}
