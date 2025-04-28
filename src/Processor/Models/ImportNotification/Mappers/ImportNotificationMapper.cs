using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImportNotificationMapper
{
    public static IpaffsDataApi.ImportPreNotification Map(ImportNotification? from)
    {
        if (from is null)
            return default!;
        var to = new IpaffsDataApi.ImportPreNotification();
        to.IpaffsId = from.IpaffsId;
        to.Etag = from.Etag;
        to.ExternalReferences = from.ExternalReferences?.Select(x => ExternalReferenceMapper.Map(x)).ToArray();
        to.ReferenceNumber = from.ReferenceNumber;
        to.Version = from.Version;
        to.UpdatedSource = from.LastUpdated;
        to.LastUpdatedBy = UserInformationMapper.Map(from.LastUpdatedBy);
        to.ImportNotificationType = ImportNotificationTypeEnumMapper.Map(from.ImportNotificationType);
        to.Replaces = from.Replaces;
        to.ReplacedBy = from.ReplacedBy;
        to.Status = ImportNotificationStatusEnumMapper.Map(from.Status);
        to.SplitConsignment = SplitConsignmentMapper.Map(from.SplitConsignment);
        to.ChildNotification = from.ChildNotification;
        to.JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(from.JourneyRiskCategorisation);
        to.IsHighRiskEuImport = from.IsHighRiskEuImport;
        to.PartOne = PartOneMapper.Map(from.PartOne);
        to.DecisionBy = UserInformationMapper.Map(from.DecisionBy);
        to.DecisionDate = from.DecisionDate;
        to.PartTwo = PartTwoMapper.Map(from.PartTwo);
        to.PartThree = PartThreeMapper.Map(from.PartThree);
        to.OfficialVeterinarian = from.OfficialVeterinarian;
        to.ConsignmentValidations = from
            .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
            .ToArray();
        to.AgencyOrganisationId = from.AgencyOrganisationId;
        to.RiskDecisionLockedOn = from.RiskDecisionLockedOn;
        to.IsRiskDecisionLocked = from.IsRiskDecisionLocked;
        to.IsBulkUploadInProgress = from.IsBulkUploadInProgress;
        to.RequestId = from.RequestId;
        to.IsCdsFullMatched = from.IsCdsFullMatched;
        to.ChedTypeVersion = from.ChedTypeVersion;
        to.IsGMRMatched = from.IsGMRMatched;
        return to;
    }
}
