using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;
using SlimMessageBus;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<object> logger, ITradeImportsDataApiClient api) : IConsumer<object>
{
    public async Task OnHandle(object received, CancellationToken cancellationToken)
    {
        var importNotification = JsonSerializer.Deserialize<ImportNotification>(JsonSerializer.Serialize(received));
        if (importNotification == null)
        {
            logger.LogWarning("Received invalid message {Received}", received);
            throw new InvalidOperationException("Received invalid message");
        }

        logger.LogInformation("Received notification {ReferenceNumber}", importNotification.ReferenceNumber);

        if (ShouldNotProcess(importNotification))
        {
            logger.LogInformation("Skipping {ReferenceNumber} due to status", importNotification.ReferenceNumber);
            return;
        }

        var to = new IpaffsDataApi.ImportPreNotification
        {
            IpaffsId = importNotification.IpaffsId,
            Etag = importNotification.Etag,
            ExternalReferences = importNotification.ExternalReferences?.Select(ExternalReferenceMapper.Map).ToArray(),
            ReferenceNumber = importNotification.ReferenceNumber,
            Version = importNotification.Version,
            UpdatedSource = importNotification.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(importNotification.LastUpdatedBy),
            ImportNotificationType = ImportNotificationTypeEnumMapper.Map(importNotification.ImportNotificationType),
            Replaces = importNotification.Replaces,
            ReplacedBy = importNotification.ReplacedBy,
            Status = ImportNotificationStatusEnumMapper.Map(importNotification.Status),
            SplitConsignment = SplitConsignmentMapper.Map(importNotification.SplitConsignment),
            ChildNotification = importNotification.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(
                importNotification.JourneyRiskCategorisation
            ),
            IsHighRiskEuImport = importNotification.IsHighRiskEuImport,
            PartOne = PartOneMapper.Map(importNotification.PartOne),
            DecisionBy = UserInformationMapper.Map(importNotification.DecisionBy),
            DecisionDate = importNotification.DecisionDate,
            PartTwo = PartTwoMapper.Map(importNotification.PartTwo),
            PartThree = PartThreeMapper.Map(importNotification.PartThree),
            OfficialVeterinarian = importNotification.OfficialVeterinarian,
            ConsignmentValidations = importNotification
                .ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map)
                .ToArray(),
            AgencyOrganisationId = importNotification.AgencyOrganisationId,
            RiskDecisionLockedOn = importNotification.RiskDecisionLockedOn,
            IsRiskDecisionLocked = importNotification.IsRiskDecisionLocked,
            IsBulkUploadInProgress = importNotification.IsBulkUploadInProgress,
            RequestId = importNotification.RequestId,
            IsCdsFullMatched = importNotification.IsCdsFullMatched,
            ChedTypeVersion = importNotification.ChedTypeVersion,
            IsGMRMatched = importNotification.IsGMRMatched,
        };

        var existingNotification = await api.GetImportPreNotification(
            importNotification.ReferenceNumber!,
            cancellationToken
        );
        if (existingNotification != null)
        {
            logger.LogInformation(
                "Updating existing notification {ReferenceNumber}",
                existingNotification.ImportPreNotification.ReferenceNumber
            );
            await api.PutImportPreNotification(
                importNotification.ReferenceNumber!,
                to,
                existingNotification.ETag,
                cancellationToken
            );
            return;
        }

        logger.LogInformation("Creating new notification {ReferenceNumber}", to.ReferenceNumber);
        await api.PutImportPreNotification(importNotification.ReferenceNumber!, to, null, cancellationToken);
    }

    private static bool ShouldNotProcess(ImportNotification notification)
    {
        return notification.Status == ImportNotificationStatus.Amend
            || notification.Status == ImportNotificationStatus.Draft
            || notification.ReferenceNumber!.StartsWith("DRAFT", StringComparison.InvariantCultureIgnoreCase);
    }
}
