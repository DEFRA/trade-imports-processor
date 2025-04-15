using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Extensions;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;
using SlimMessageBus;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;
using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<NotificationConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<JsonElement>
{
    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var newNotification = received.Deserialize<ImportNotification>();
        if (newNotification == null)
        {
            logger.LogWarning("Received invalid message {Received}", received);
            throw new InvalidOperationException("Received invalid message");
        }

        logger.LogInformation("Received notification {ReferenceNumber}", newNotification.ReferenceNumber);

        if (ShouldNotProcess(newNotification))
        {
            logger.LogInformation("Skipping {ReferenceNumber} due to status", newNotification.ReferenceNumber);
            return;
        }

        var to = new IpaffsDataApi.ImportPreNotification
        {
            IpaffsId = newNotification.IpaffsId,
            Etag = newNotification.Etag,
            ExternalReferences = newNotification.ExternalReferences?.Select(ExternalReferenceMapper.Map).ToArray(),
            ReferenceNumber = newNotification.ReferenceNumber,
            Version = newNotification.Version,
            UpdatedSource = newNotification.LastUpdated,
            LastUpdatedBy = UserInformationMapper.Map(newNotification.LastUpdatedBy),
            ImportNotificationType = ImportNotificationTypeEnumMapper.Map(newNotification.ImportNotificationType),
            Replaces = newNotification.Replaces,
            ReplacedBy = newNotification.ReplacedBy,
            Status = ImportNotificationStatusEnumMapper.Map(newNotification.Status),
            SplitConsignment = SplitConsignmentMapper.Map(newNotification.SplitConsignment),
            ChildNotification = newNotification.ChildNotification,
            JourneyRiskCategorisation = JourneyRiskCategorisationResultMapper.Map(
                newNotification.JourneyRiskCategorisation
            ),
            IsHighRiskEuImport = newNotification.IsHighRiskEuImport,
            PartOne = PartOneMapper.Map(newNotification.PartOne),
            DecisionBy = UserInformationMapper.Map(newNotification.DecisionBy),
            DecisionDate = newNotification.DecisionDate,
            PartTwo = PartTwoMapper.Map(newNotification.PartTwo),
            PartThree = PartThreeMapper.Map(newNotification.PartThree),
            OfficialVeterinarian = newNotification.OfficialVeterinarian,
            ConsignmentValidations = newNotification
                .ConsignmentValidations?.Select(ValidationMessageCodeMapper.Map)
                .ToArray(),
            AgencyOrganisationId = newNotification.AgencyOrganisationId,
            RiskDecisionLockedOn = newNotification.RiskDecisionLockedOn,
            IsRiskDecisionLocked = newNotification.IsRiskDecisionLocked,
            IsBulkUploadInProgress = newNotification.IsBulkUploadInProgress,
            RequestId = newNotification.RequestId,
            IsCdsFullMatched = newNotification.IsCdsFullMatched,
            ChedTypeVersion = newNotification.ChedTypeVersion,
            IsGMRMatched = newNotification.IsGMRMatched,
        };

        var existingNotification = await api.GetImportPreNotification(
            newNotification.ReferenceNumber!,
            cancellationToken
        );

        if (existingNotification == null)
        {
            logger.LogInformation("Creating new notification {ReferenceNumber}", to.ReferenceNumber);
            await api.PutImportPreNotification(newNotification.ReferenceNumber!, to, null, cancellationToken);
            return;
        }

        if (
            NewNotificationIsOlderThanExistingNotification(newNotification, existingNotification.ImportPreNotification)
            || GoingBackIntoInProgress(newNotification, existingNotification.ImportPreNotification)
        )
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} because new notification is going back in time/progress",
                newNotification.ReferenceNumber!
            );
            return;
        }

        logger.LogInformation(
            "Updating existing notification {ReferenceNumber}",
            existingNotification.ImportPreNotification.ReferenceNumber
        );

        await api.PutImportPreNotification(
            newNotification.ReferenceNumber!,
            to,
            existingNotification.ETag,
            cancellationToken
        );
    }

    private static bool NewNotificationIsOlderThanExistingNotification(
        ImportNotification newNotification,
        IpaffsDataApi.ImportPreNotification existingNotification
    )
    {
        return newNotification.LastUpdated.TrimMicroseconds() < existingNotification.UpdatedSource.TrimMicroseconds();
    }

    private static bool ShouldNotProcess(ImportNotification notification)
    {
        return notification.Status == ImportNotificationStatus.Amend
            || notification.Status == ImportNotificationStatus.Draft
            || notification.ReferenceNumber!.StartsWith("DRAFT", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool GoingBackIntoInProgress(
        ImportNotification newNotification,
        IpaffsDataApi.ImportPreNotification existingNotification
    )
    {
        return newNotification.Status == ImportNotificationStatus.InProgress
            && existingNotification.Status
                is IpaffsDataApi.ImportNotificationStatus.Validated
                    or IpaffsDataApi.ImportNotificationStatus.Rejected
                    or IpaffsDataApi.ImportNotificationStatus.PartiallyRejected;
    }
}
