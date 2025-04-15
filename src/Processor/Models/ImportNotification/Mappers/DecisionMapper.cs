using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionMapper
{
    public static IpaffsDataApi.Decision Map(Decision? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.Decision();
        to.ConsignmentAcceptable = from?.ConsignmentAcceptable;
        to.NotAcceptableAction = DecisionNotAcceptableActionEnumMapper.Map(from?.NotAcceptableAction);
        to.NotAcceptableActionDestructionReason = DecisionNotAcceptableActionDestructionReasonEnumMapper.Map(
            from?.NotAcceptableActionDestructionReason
        );
        to.NotAcceptableActionEntryRefusalReason = DecisionNotAcceptableActionEntryRefusalReasonEnumMapper.Map(
            from?.NotAcceptableActionEntryRefusalReason
        );
        to.NotAcceptableActionQuarantineImposedReason =
            DecisionNotAcceptableActionQuarantineImposedReasonEnumMapper.Map(
                from?.NotAcceptableActionQuarantineImposedReason
            );
        to.NotAcceptableActionSpecialTreatmentReason = DecisionNotAcceptableActionSpecialTreatmentReasonEnumMapper.Map(
            from?.NotAcceptableActionSpecialTreatmentReason
        );
        to.NotAcceptableActionIndustrialProcessingReason =
            DecisionNotAcceptableActionIndustrialProcessingReasonEnumMapper.Map(
                from?.NotAcceptableActionIndustrialProcessingReason
            );
        to.NotAcceptableActionReDispatchReason = DecisionNotAcceptableActionReDispatchReasonEnumMapper.Map(
            from?.NotAcceptableActionReDispatchReason
        );
        to.NotAcceptableActionUseForOtherPurposesReason =
            DecisionNotAcceptableActionUseForOtherPurposesReasonEnumMapper.Map(
                from?.NotAcceptableActionUseForOtherPurposesReason
            );
        to.NotAcceptableDestructionReason = from?.NotAcceptableDestructionReason;
        to.NotAcceptableActionOtherReason = from?.NotAcceptableActionOtherReason;
        to.NotAcceptableActionByDate = from?.NotAcceptableActionByDate;
        to.ChedppNotAcceptableReasons = from
            ?.ChedppNotAcceptableReasons?.Select(x => ChedppNotAcceptableReasonMapper.Map(x))
            .ToArray();
        to.NotAcceptableReasons = from?.NotAcceptableReasons;
        to.NotAcceptableCountry = from?.NotAcceptableCountry;
        to.NotAcceptableEstablishment = from?.NotAcceptableEstablishment;
        to.NotAcceptableOtherReason = from?.NotAcceptableOtherReason;
        to.DetailsOfControlledDestinations = PartyMapper.Map(from?.DetailsOfControlledDestinations);
        to.SpecificWarehouseNonConformingConsignment = DecisionSpecificWarehouseNonConformingConsignmentEnumMapper.Map(
            from?.SpecificWarehouseNonConformingConsignment
        );
        to.TemporaryDeadline = from?.TemporaryDeadline;
        to.ConsignmentDecision = DecisionDecisionEnumMapper.Map(from?.DecisionEnum);
        to.FreeCirculationPurpose = DecisionFreeCirculationPurposeEnumMapper.Map(from?.FreeCirculationPurpose);
        to.DefinitiveImportPurpose = DecisionDefinitiveImportPurposeEnumMapper.Map(from?.DefinitiveImportPurpose);
        to.IfChanneledOption = DecisionIfChanneledOptionEnumMapper.Map(from?.IfChanneledOption);
        to.CustomWarehouseRegisteredNumber = from?.CustomWarehouseRegisteredNumber;
        to.FreeWarehouseRegisteredNumber = from?.FreeWarehouseRegisteredNumber;
        to.ShipName = from?.ShipName;
        to.ShipPortOfExit = from?.ShipPortOfExit;
        to.ShipSupplierRegisteredNumber = from?.ShipSupplierRegisteredNumber;
        to.TranshipmentBip = from?.TranshipmentBip;
        to.TranshipmentThirdCountry = from?.TranshipmentThirdCountry;
        to.TransitExitBip = from?.TransitExitBip;
        to.TransitThirdCountry = from?.TransitThirdCountry;
        to.TransitDestinationThirdCountry = from?.TransitDestinationThirdCountry;
        to.TemporaryExitBip = from?.TemporaryExitBip;
        to.HorseReentry = from?.HorseReentry;
        to.TranshipmentEuOrThirdCountry = from?.TranshipmentEuOrThirdCountry;
        return to;
    }
}
