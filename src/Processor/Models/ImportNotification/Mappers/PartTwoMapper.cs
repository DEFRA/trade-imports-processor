using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartTwoMapper
{
    public static IpaffsDataApi.PartTwo Map(PartTwo? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.PartTwo
        {
            Decision = DecisionMapper.Map(from.Decision),
            ConsignmentCheck = ConsignmentCheckMapper.Map(from.ConsignmentCheck),
            ImpactOfTransportOnAnimals = ImpactOfTransportOnAnimalsMapper.Map(from.ImpactOfTransportOnAnimals),
            LaboratoryTestsRequired = from.LaboratoryTestsRequired,
            LaboratoryTests = LaboratoryTestsMapper.Map(from.LaboratoryTests),
            ResealedContainersIncluded = from.ResealedContainersIncluded,
            ResealedContainers = from.ResealedContainers,
            ResealedContainersMappings = from.ResealedContainersMappings?.Select(SealContainerMapper.Map).ToArray(),
            ControlAuthority = ControlAuthorityMapper.Map(from.ControlAuthority),
            ControlledDestination = EconomicOperatorMapper.Map(from.ControlledDestination),
            BipLocalReferenceNumber = from.BipLocalReferenceNumber,
            SignedOnBehalfOf = from.SignedOnBehalfOf,
            OnwardTransportation = from.OnwardTransportation,
            ConsignmentValidations = from.ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map).ToArray(),
            CheckedOn = from.CheckDate,
            AccompanyingDocuments = from.AccompanyingDocuments?.Select(AccompanyingDocumentMapper.Map).ToArray(),
            CommodityChecks = from.CommodityChecks?.Select(CommodityCheckMapper.Map).ToArray(),
            PhsiAutoCleared = from.PhsiAutoCleared,
            HmiAutoCleared = from.HmiAutoCleared,
            InspectionRequired = from.InspectionRequired,
            InspectionOverride = InspectionOverrideMapper.Map(from.InspectionOverride),
            AutoClearedOn = from.AutoClearedDateTime,
            RequestAmendmentAdditionalDetails = from.RequestAmendmentAdditionalDetails,
        };

        return to;
    }
}
