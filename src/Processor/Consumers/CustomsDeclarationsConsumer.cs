using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Models.ClearanceRequest;
using SlimMessageBus;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class CustomsDeclarationsConsumer(ILogger<CustomsDeclarationsConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<JsonElement>
{
    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        // Check header for message type, assume ClearanceRequest for now
        var clearanceRequest = received.Deserialize<ClearanceRequest>();
        if (clearanceRequest == null)
        {
            logger.LogWarning("Received invalid message {Received}", received);
            throw new InvalidOperationException("Received invalid message");
        }

        logger.LogInformation("Received clearance request");

        var mrn = clearanceRequest.Header.EntryReference!;

        var to = new DataApiCustomsDeclaration.ClearanceRequest
        {
            ExternalCorrelationId = clearanceRequest.ServiceHeader.CorrelationId,
            MessageSentAt = clearanceRequest.ServiceHeader.ServiceCallTimestamp,
            ExternalVersion = clearanceRequest.Header.EntryVersionNumber,
            PreviousExternalVersion = clearanceRequest.Header.PreviousVersionNumber,
            DeclarationUcr = clearanceRequest.Header.DeclarationUcr,
            DeclarationPartNumber = clearanceRequest.Header.DeclarationPartNumber,
            DeclarationType = clearanceRequest.Header.DeclarationType,
            ArrivesAt = clearanceRequest.Header.ArrivalDateTime,
            SubmitterTurn = clearanceRequest.Header.SubmitterTurn,
            DeclarantId = clearanceRequest.Header.DeclarantId,
            DeclarantName = clearanceRequest.Header.DeclarantName,
            DispatchCountryCode = clearanceRequest.Header.DispatchCountryCode,
            GoodsLocationCode = clearanceRequest.Header.GoodsLocationCode,
            MasterUcr = clearanceRequest.Header.MasterUcr,
            Commodities = clearanceRequest
                .Items.Select(item => new DataApiCustomsDeclaration.Commodity
                {
                    ItemNumber = item.ItemNumber,
                    CustomsProcedureCode = item.CustomsProcedureCode,
                    TaricCommodityCode = item.TaricCommodityCode,
                    GoodsDescription = item.GoodsDescription,
                    ConsigneeId = item.ConsigneeId,
                    ConsigneeName = item.ConsigneeName,
                    NetMass = item.ItemNetMass,
                    SupplementaryUnits = item.ItemSupplementaryUnits,
                    ThirdQuantity = item.ItemThirdQuantity,
                    OriginCountryCode = item.ItemOriginCountryCode,
                    Documents = item
                        .Documents?.Select(doc => new DataApiCustomsDeclaration.ImportDocument
                        {
                            DocumentCode = doc.DocumentCode,
                            DocumentReference =
                                doc.DocumentReference != null
                                    ? new DataApiCustomsDeclaration.ImportDocumentReference(doc.DocumentReference)
                                    : null,
                            DocumentStatus = doc.DocumentStatus,
                            DocumentControl = doc.DocumentControl,
                            DocumentQuantity = doc.DocumentQuantity,
                        })
                        .ToArray(),
                })
                .ToArray(),
        };

        var customsDeclaration = new DataApiCustomsDeclaration.CustomsDeclaration { ClearanceRequest = to };

        var existingCustomsDeclaration = await api.GetCustomsDeclaration(mrn, cancellationToken);
        if (existingCustomsDeclaration?.ClearanceRequest == null)
        {
            logger.LogInformation("Creating new customs declaration {Mrn}", mrn);
            await api.PutCustomsDeclaration(mrn, customsDeclaration, null, cancellationToken);
            return;
        }

        logger.LogInformation("Updating existing customs declaration {Mrn}", mrn);
        await api.PutCustomsDeclaration(mrn, customsDeclaration, existingCustomsDeclaration.ETag, cancellationToken);
    }
}
