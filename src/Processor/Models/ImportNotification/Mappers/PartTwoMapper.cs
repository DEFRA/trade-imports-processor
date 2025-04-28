using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class PartTwoMapper
{
    public static IpaffsDataApi.PartTwo Map(PartTwo? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.PartTwo();
        to.Decision = DecisionMapper.Map(from.Decision);
        to.ConsignmentCheck = ConsignmentCheckMapper.Map(from.ConsignmentCheck);
        to.ImpactOfTransportOnAnimals = ImpactOfTransportOnAnimalsMapper.Map(from.ImpactOfTransportOnAnimals);
        to.LaboratoryTestsRequired = from.LaboratoryTestsRequired;
        to.LaboratoryTests = LaboratoryTestsMapper.Map(from.LaboratoryTests);
        to.ResealedContainersIncluded = from.ResealedContainersIncluded;
        to.ResealedContainers = from.ResealedContainers;
        to.ResealedContainersMappings = from
            .ResealedContainersMappings?.Select(x => SealContainerMapper.Map(x))
            .ToArray();
        to.ControlAuthority = ControlAuthorityMapper.Map(from.ControlAuthority);
        to.ControlledDestination = EconomicOperatorMapper.Map(from.ControlledDestination);
        to.BipLocalReferenceNumber = from.BipLocalReferenceNumber;
        to.SignedOnBehalfOf = from.SignedOnBehalfOf;
        to.OnwardTransportation = from.OnwardTransportation;
        to.ConsignmentValidations = from
            .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
            .ToArray();
        to.CheckedOn = from.CheckDate;
        to.AccompanyingDocuments = from.AccompanyingDocuments?.Select(x => AccompanyingDocumentMapper.Map(x)).ToArray();
        to.PhsiAutoCleared = from.PhsiAutoCleared;
        to.HmiAutoCleared = from.HmiAutoCleared;
        to.InspectionRequired = InspectionRequiredEnumMapper.Map(from.InspectionRequired);
        to.InspectionOverride = InspectionOverrideMapper.Map(from.InspectionOverride);
        to.AutoClearedOn = from.AutoClearedDateTime;
        return to;
    }
}
