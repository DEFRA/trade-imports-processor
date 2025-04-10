using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;
using SlimMessageBus;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<object> logger, ITradeImportsDataApiClient api) : IConsumer<object>
{
    public async Task OnHandle(object received, CancellationToken cancellationToken)
    {
        var from = JsonSerializer.Deserialize<ImportNotification>(JsonSerializer.Serialize(received));
        if (from == null)
        {
            logger.LogWarning("Received invalid message {Received}", received);
            throw new InvalidOperationException("Received invalid message");
        }

        logger.LogInformation("Received notification {ReferenceNumber}", from.ReferenceNumber);

        var to = new IpaffsDataApi.ImportNotification
        {
            IpaffsId = from.IpaffsId,
            Etag = from.Etag,
            ExternalReferences = from.ExternalReferences?.Select(ExternalReferenceMapper.Map).ToArray(),
            ReferenceNumber = from.ReferenceNumber,
            Version = from.Version,
            UpdatedSource = from.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(from.LastUpdatedBy),
            ImportNotificationType = ImportNotificationTypeEnumMapper.Map(from.ImportNotificationType),
            Replaces = from.Replaces,
            ReplacedBy = from.ReplacedBy,
            Status = ImportNotificationStatusEnumMapper.Map(from.Status),
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
            ConsignmentValidations = from.ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map).ToArray(),
            AgencyOrganisationId = from.AgencyOrganisationId,
            RiskDecisionLockedOn = from.RiskDecisionLockedOn,
            IsRiskDecisionLocked = from.IsRiskDecisionLocked,
            IsBulkUploadInProgress = from.IsBulkUploadInProgress,
            RequestId = from.RequestId,
            IsCdsFullMatched = from.IsCdsFullMatched,
            ChedTypeVersion = from.ChedTypeVersion,
            IsGMRMatched = from.IsGMRMatched,
        };

        var existingNotification = await api.GetImportNotification(from.ReferenceNumber!, cancellationToken);
        if (existingNotification != null)
        {
            logger.LogInformation(
                "Updating existing notification {ReferenceNumber}",
                existingNotification.Data.ReferenceNumber
            );
            await api.PutImportNotification(from.ReferenceNumber!, to, existingNotification.ETag, cancellationToken);
            return;
        }

        logger.LogInformation("Creating new notification {ReferenceNumber}", to.ReferenceNumber);
        await api.PutImportNotification(from.ReferenceNumber!, to, null, cancellationToken);
    }
}
