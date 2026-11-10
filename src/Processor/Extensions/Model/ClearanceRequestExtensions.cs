using BtmsGateway.Contract.CustomsDeclarations;
using Defra.TradeImportsProcessor.Processor.Exceptions;
using DataApiCustomsDeclaration = Defra.TradeImportsDataApi.Domain.CustomsDeclaration;

namespace Defra.TradeImportsProcessor.Processor.Extensions.Model
{
    public static class ClearanceRequestExtensions
    {
        public static DataApiCustomsDeclaration.ClearanceRequest ToDataApiClearanceRequest(
            this ClearanceRequest clearanceRequest
        )
        {
            if (clearanceRequest.ServiceHeader.ServiceCallTimestamp == null)
            {
                throw new CustomsDeclarationMessageMissingFieldException<ClearanceRequest>(
                    clearanceRequest.ServiceHeader.ServiceCallTimestamp
                );
            }

            return new DataApiCustomsDeclaration.ClearanceRequest
            {
                ExternalCorrelationId = clearanceRequest.ServiceHeader.CorrelationId,
                MessageSentAt = clearanceRequest.ServiceHeader.ServiceCallTimestamp.Value,
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
                    .Items.Select(
                        (Item item) =>
                            new DataApiCustomsDeclaration.Commodity
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
                                    .Checks?.Select(
                                        (Check check) =>
                                            new DataApiCustomsDeclaration.CommodityCheck
                                            {
                                                CheckCode = check.CheckCode,
                                                DepartmentCode = check.DepartmentCode,
                                            }
                                    )
                                    .ToArray(),
                                Documents = item
                                    .Documents?.Select(
                                        (Document doc) =>
                                            new DataApiCustomsDeclaration.ImportDocument
                                            {
                                                DocumentCode = doc.DocumentCode,
                                                DocumentReference =
                                                    doc.DocumentReference != null
                                                        ? new DataApiCustomsDeclaration.ImportDocumentReference(
                                                            doc.DocumentReference.TrimEnd()
                                                        )
                                                        : null,
                                                DocumentStatus = doc.DocumentStatus,
                                                DocumentControl = doc.DocumentControl,
                                                DocumentQuantity = doc.DocumentQuantity,
                                            }
                                    )
                                    .ToArray(),
                            }
                    )
                    .ToArray(),
            };
        }
    }
}
