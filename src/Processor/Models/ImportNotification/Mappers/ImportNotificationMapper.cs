using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImportNotificationMapper
{
    public static IpaffsDataApi.ImportPreNotification Map(ImportNotification? from)
    {
        if (from is null)
            return null!;

        var to = new IpaffsDataApi.ImportPreNotification
        {
            IpaffsId = from.IpaffsId,
            Etag = from.Etag,
            ExternalReferences = from.ExternalReferences?.Select(x => ExternalReferenceMapper.Map(x)).ToArray(),
            ReferenceNumber = from.ReferenceNumber,
            Version = from.Version,
            UpdatedSource = from.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(from.LastUpdatedBy),
            ImportNotificationType = from.ImportNotificationType,
            Replaces = from.Replaces,
            ReplacedBy = from.ReplacedBy,
            Status = from.Status,
            SplitConsignment = SplitConsignmentMapper.Map(from.SplitConsignment),
            ChildNotification = from.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(from.JourneyRiskCategorisation),
            IsHighRiskEuImport = from.IsHighRiskEuImport,
            PartOne = PartOneMapper.Map(from.PartOne),
            DecisionBy = UserInformationMapper.Map(from.DecisionBy),
            DecisionDate = from.DecisionDate,
            PartTwo = PartTwoMapper.Map(from.PartTwo),
            PartThree = PartThreeMapper.Map(from.PartThree),
            OfficialVeterinarian = from.OfficialVeterinarian,
            ConsignmentValidations = from
                .ConsignmentValidations?.Select(x => ValidationMessageCodeMapper.Map(x))
                .ToArray(),
            AgencyOrganisationId = from.AgencyOrganisationId,
            RiskDecisionLockedOn = from.RiskDecisionLockedOn,
            IsRiskDecisionLocked = from.IsRiskDecisionLocked,
            IsBulkUploadInProgress = from.IsBulkUploadInProgress,
            RequestId = from.RequestId,
            IsCdsFullMatched = from.IsCdsFullMatched,
            ChedTypeVersion = from.ChedTypeVersion,
            IsGMRMatched = from.IsGMRMatched,
        };

        return to;
    }
}
