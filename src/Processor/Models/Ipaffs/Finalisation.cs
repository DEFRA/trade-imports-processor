using System.Text.Json.Serialization;
using CustomsDeclarationFinalisation = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.Finalisation;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class Finalisation(string mrn, CustomsDeclarationFinalisation finalisation)
{
    [JsonPropertyName("serviceHeader")]
    public ServiceHeader ServiceHeader { get; set; } =
        new()
        {
            SourceSystem = "CDS",
            DestinationSystem = "ALVS",
            CorrelationId = finalisation.ExternalCorrelationId ?? string.Empty,
            ServiceCallTimestamp = finalisation.MessageSentAt,
        };

    [JsonPropertyName("header")]
    public FinalisationHeader Header { get; set; } =
        new()
        {
            EntryReference = mrn,
            EntryVersionNumber = finalisation.ExternalVersion,
            DecisionNumber = finalisation.DecisionNumber,
            FinalState = finalisation.FinalState,
            ManualAction = finalisation.IsManualRelease ? "Y" : "N",
        };
}
