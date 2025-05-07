using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DecisionMapper
{
    public static IpaffsDataApi.Decision Map(Decision? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.Decision
        {
            ConsignmentAcceptable = from.ConsignmentAcceptable,
            NotAcceptableAction = from.NotAcceptableAction,
            NotAcceptableActionDestructionReason = from.NotAcceptableActionDestructionReason,
            NotAcceptableActionEntryRefusalReason = from.NotAcceptableActionEntryRefusalReason,
            NotAcceptableActionQuarantineImposedReason = from.NotAcceptableActionQuarantineImposedReason,
            NotAcceptableActionSpecialTreatmentReason = from.NotAcceptableActionSpecialTreatmentReason,
            NotAcceptableActionIndustrialProcessingReason = from.NotAcceptableActionIndustrialProcessingReason,
            NotAcceptableActionReDispatchReason = from.NotAcceptableActionReDispatchReason,
            NotAcceptableActionUseForOtherPurposesReason = from.NotAcceptableActionUseForOtherPurposesReason,
            NotAcceptableDestructionReason = from.NotAcceptableDestructionReason,
            NotAcceptableActionOtherReason = from.NotAcceptableActionOtherReason,
            NotAcceptableActionByDate = from.NotAcceptableActionByDate,
            ChedppNotAcceptableReasons = from
                .ChedppNotAcceptableReasons?.Select(x => ChedppNotAcceptableReasonMapper.Map(x))
                .ToArray(),
            NotAcceptableReasons = from.NotAcceptableReasons,
            NotAcceptableCountry = from.NotAcceptableCountry,
            NotAcceptableEstablishment = from.NotAcceptableEstablishment,
            NotAcceptableOtherReason = from.NotAcceptableOtherReason,
            DetailsOfControlledDestinations = PartyMapper.Map(from.DetailsOfControlledDestinations),
            SpecificWarehouseNonConformingConsignment = from.SpecificWarehouseNonConformingConsignment,
            TemporaryDeadline = from.TemporaryDeadline,
            ConsignmentDecision = from.DecisionEnum,
            FreeCirculationPurpose = from.FreeCirculationPurpose,
            DefinitiveImportPurpose = from.DefinitiveImportPurpose,
            IfChanneledOption = from.IfChanneledOption,
            CustomWarehouseRegisteredNumber = from.CustomWarehouseRegisteredNumber,
            FreeWarehouseRegisteredNumber = from.FreeWarehouseRegisteredNumber,
            ShipName = from.ShipName,
            ShipPortOfExit = from.ShipPortOfExit,
            ShipSupplierRegisteredNumber = from.ShipSupplierRegisteredNumber,
            TranshipmentBip = from.TranshipmentBip,
            TranshipmentThirdCountry = from.TranshipmentThirdCountry,
            TransitExitBip = from.TransitExitBip,
            TransitThirdCountry = from.TransitThirdCountry,
            TransitDestinationThirdCountry = from.TransitDestinationThirdCountry,
            TemporaryExitBip = from.TemporaryExitBip,
            HorseReentry = from.HorseReentry,
            TranshipmentEuOrThirdCountry = from.TranshipmentEuOrThirdCountry,
        };

        return to;
    }
}
