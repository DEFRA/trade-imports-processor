using System.Text.Json.Serialization;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Models.CustomsDeclarations;

public class ClearanceRequest
{
    [JsonPropertyName("header")]
    public required ClearanceRequestHeader Header { get; init; }

    [JsonPropertyName("serviceHeader")]
    public required ServiceHeader ServiceHeader { get; init; }

    [JsonPropertyName("items")]
    public required Item[] Items { get; init; }

    public static explicit operator DataApiCustomsDeclaration.ClearanceRequest(ClearanceRequest clearanceRequest)
    {
        return new DataApiCustomsDeclaration.ClearanceRequest
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
                    Checks = item
                        .Checks?.Select(check => new DataApiCustomsDeclaration.CommodityCheck
                        {
                            CheckCode = check.CheckCode,
                            DepartmentCode = check.DepartmentCode,
                        })
                        .ToArray(),
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
    }
}
