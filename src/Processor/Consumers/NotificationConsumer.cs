using System.Collections.Frozen;
using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<NotificationConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<JsonElement>
{
    /// <summary>
    /// See https://eaflood.atlassian.net/wiki/spaces/ALVS/pages/5352587513/IPAFFS+Notification+States
    /// </summary>
    private static readonly HashSet<(string, string)> s_statusStateMachine =
    [
        // Amend
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Deleted),
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Submitted),
        // Submitted
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Submitted),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.InProgress),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Deleted),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Validated), // auto clearance process
        // In progress
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.InProgress),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Validated),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Cancelled),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Rejected),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Replaced),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.PartiallyRejected),
        // Partially rejected
        (ImportNotificationStatus.PartiallyRejected, ImportNotificationStatus.PartiallyRejected),
        (ImportNotificationStatus.PartiallyRejected, ImportNotificationStatus.SplitConsignment),
        // Validated
        (ImportNotificationStatus.Validated, ImportNotificationStatus.Validated),
        (ImportNotificationStatus.Validated, ImportNotificationStatus.Replaced),
        (ImportNotificationStatus.Validated, ImportNotificationStatus.Cancelled),
    ];

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var newNotification = received.Deserialize<ImportNotification>();
        if (newNotification == null)
        {
            throw new InvalidOperationException("Received invalid message, deserialised as null");
        }

        logger.LogInformation("Received notification {ReferenceNumber}", newNotification.ReferenceNumber);

        if (IsInvalidStatus(newNotification))
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} due to status {Status}",
                newNotification.ReferenceNumber,
                newNotification.Status
            );

            return;
        }

        var dataApiImportPreNotification = (DataApiIpaffs.ImportPreNotification)newNotification;

        var existingNotification = await api.GetImportPreNotification(
            newNotification.ReferenceNumber,
            cancellationToken
        );

        if (
            existingNotification != null
            && !ShouldProcess(dataApiImportPreNotification, existingNotification.ImportPreNotification)
        )
        {
            return;
        }

        if (existingNotification == null)
        {
            await CreateNotification(dataApiImportPreNotification, newNotification, cancellationToken);

            return;
        }

        await UpdateNotification(
            existingNotification,
            newNotification,
            dataApiImportPreNotification,
            cancellationToken
        );
    }

    private async Task UpdateNotification(
        ImportPreNotificationResponse existingNotification,
        ImportNotification newNotification,
        DataApiIpaffs.ImportPreNotification dataApiImportPreNotification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Updating existing notification {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}, version {Version}",
            existingNotification.ImportPreNotification.ReferenceNumber,
            dataApiImportPreNotification.Status,
            dataApiImportPreNotification.UpdatedSource,
            dataApiImportPreNotification.Version
        );

        await api.PutImportPreNotification(
            newNotification.ReferenceNumber,
            dataApiImportPreNotification,
            existingNotification.ETag,
            cancellationToken
        );
    }

    private async Task CreateNotification(
        DataApiIpaffs.ImportPreNotification dataApiImportPreNotification,
        ImportNotification newNotification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Creating new notification {ReferenceNumber}, status {Status}, updated source {UpdatedSource:O}, version {Version}",
            dataApiImportPreNotification.ReferenceNumber,
            dataApiImportPreNotification.Status,
            dataApiImportPreNotification.UpdatedSource,
            dataApiImportPreNotification.Version
        );

        await api.PutImportPreNotification(
            newNotification.ReferenceNumber,
            dataApiImportPreNotification,
            null,
            cancellationToken
        );
    }

    private bool ShouldProcess(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        if (
            newNotification.Status == existingNotification.Status
            && NewNotificationIsOlderThanExistingNotification(newNotification, existingNotification)
        )
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} because new notification of the same status {Status} is older: {NewTime:O} < {OldTime:O}",
                newNotification.ReferenceNumber,
                newNotification.Status,
                newNotification.UpdatedSource,
                existingNotification.UpdatedSource
            );

            return false;
        }

        if (IsStateTransitionAllowed(newNotification, existingNotification))
            return true;

        logger.LogInformation(
            "Skipping {ReferenceNumber} because new notification is going backwards: {NewStatus} < {OldStatus} or {NewTime:O} < {OldTime:O}",
            newNotification.ReferenceNumber,
            newNotification.Status,
            existingNotification.Status,
            newNotification.UpdatedSource,
            existingNotification.UpdatedSource
        );

        return false;
    }

    private static bool NewNotificationIsOlderThanExistingNotification(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        return newNotification.UpdatedSource.TrimMicroseconds()
            <= existingNotification.UpdatedSource.TrimMicroseconds();
    }

    private static bool IsInvalidStatus(ImportNotification notification)
    {
        return notification.Status == ImportNotificationStatus.Modify
            || notification.Status == ImportNotificationStatus.Draft
            || notification.ReferenceNumber.StartsWith("DRAFT", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsStateTransitionAllowed(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    ) => s_statusStateMachine.Contains((existingNotification.Status!, newNotification.Status!));
}
