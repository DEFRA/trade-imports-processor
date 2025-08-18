using System.Text.Json.Serialization;
using Defra.TradeImportsDataApi.Domain.CustomsDeclaration;
using CustomsDeclarationClearanceRequest = Defra.TradeImportsDataApi.Domain.CustomsDeclaration.ClearanceRequest;

namespace Defra.TradeImportsProcessor.Processor.Models.Ipaffs;

public class ClearanceRequest(string mrn, CustomsDeclarationClearanceRequest clearanceRequest)
{
    [JsonPropertyName("serviceHeader")]
    public ServiceHeader ServiceHeader { get; set; } =
        new()
        {
            SourceSystem = "CDS",
            DestinationSystem = "ALVS",
            CorrelationId = clearanceRequest.ExternalCorrelationId ?? string.Empty,
            ServiceCallTimestamp = clearanceRequest.MessageSentAt,
        };

    [JsonPropertyName("header")]
    public ClearanceRequestHeader Header { get; set; } =
        new()
        {
            EntryReference = mrn,
            EntryVersionNumber = clearanceRequest.ExternalVersion,
            PreviousVersionNumber = clearanceRequest.PreviousExternalVersion,
            DeclarationUcr = clearanceRequest.DeclarationUcr,
            DeclarationPartNumber = clearanceRequest.DeclarationPartNumber,
            DeclarationType = clearanceRequest.DeclarationType,
            ArrivalDateTime = clearanceRequest.ArrivesAt?.ToString("yyyyMMddHHmm"),
            SubmitterTurn = clearanceRequest.SubmitterTurn,
            DeclarantId = clearanceRequest.DeclarantId,
            DeclarantName = clearanceRequest.DeclarantName,
            DispatchCountryCode = clearanceRequest.DispatchCountryCode,
            GoodsLocationCode = clearanceRequest.GoodsLocationCode,
            MasterUcr = clearanceRequest.MasterUcr,
        };

    [JsonPropertyName("items")]
    public ClearanceRequestItem[] Items { get; set; } = MapItems(clearanceRequest.Commodities);

    private static ClearanceRequestItem[] MapItems(Commodity[]? commodities)
    {
        if (commodities is null || commodities.Length == 0)
            return [];

        return commodities
            .Select(commodity => new ClearanceRequestItem
            {
                ItemNumber = commodity.ItemNumber,
                CustomsProcedureCode = commodity.CustomsProcedureCode,
                TaricCommodityCode = commodity.TaricCommodityCode,
                GoodsDescription = commodity.GoodsDescription,
                ConsigneeId = commodity.ConsigneeId,
                ConsigneeName = commodity.ConsigneeName,
                ItemNetMass = commodity.NetMass,
                ItemSupplementaryUnits = commodity.SupplementaryUnits,
                ItemThirdQuantity = commodity.ThirdQuantity,
                ItemOriginCountryCode = commodity.OriginCountryCode,
                Documents =
                    commodity
                        .Documents?.Select(document => new Document
                        {
                            DocumentCode = document.DocumentCode,
                            DocumentReference = document.DocumentReference?.Value,
                            DocumentStatus = document.DocumentStatus,
                            DocumentControl = document.DocumentControl,
                            DocumentQuantity = document.DocumentQuantity,
                        })
                        .ToArray() ?? [],
                Checks =
                    commodity
                        .Checks?.Select(check => new ClearanceRequestCheck
                        {
                            CheckCode = check.CheckCode,
                            DepartmentCode = check.DepartmentCode,
                        })
                        .ToArray() ?? [],
            })
            .ToArray();
    }
}
