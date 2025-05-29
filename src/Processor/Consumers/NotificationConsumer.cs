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
    private static readonly FrozenDictionary<string, int> s_statusPriority = new Dictionary<string, int>
    {
        { ImportNotificationStatus.Draft, 0 },
        { ImportNotificationStatus.Deleted, 0 },
        { ImportNotificationStatus.Amend, 0 },
        { ImportNotificationStatus.Submitted, 0 },
        { ImportNotificationStatus.InProgress, 1 },
        { ImportNotificationStatus.Cancelled, 2 },
        { ImportNotificationStatus.PartiallyRejected, 2 },
        { ImportNotificationStatus.Rejected, 2 },
        { ImportNotificationStatus.Validated, 2 },
        { ImportNotificationStatus.SplitConsignment, 3 },
        { ImportNotificationStatus.Replaced, 3 },
    }.ToFrozenDictionary();

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var newNotification = received.Deserialize<ImportNotification>();
        if (newNotification == null)
        {
            logger.LogWarning("Received invalid message {Received}", received);
            throw new InvalidOperationException("Received invalid message");
        }

        logger.LogInformation("Received notification {ReferenceNumber}", newNotification.ReferenceNumber);

        if (IsInvalidStatus(newNotification))
        {
            logger.LogInformation("Skipping {ReferenceNumber} due to status", newNotification.ReferenceNumber);
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
            logger.LogInformation(
                "Creating new notification {ReferenceNumber}",
                dataApiImportPreNotification.ReferenceNumber
            );
            await api.PutImportPreNotification(
                newNotification.ReferenceNumber,
                dataApiImportPreNotification,
                null,
                cancellationToken
            );
            return;
        }

        logger.LogInformation(
            "Updating existing notification {ReferenceNumber}",
            existingNotification.ImportPreNotification.ReferenceNumber
        );

        await api.PutImportPreNotification(
            newNotification.ReferenceNumber,
            dataApiImportPreNotification,
            existingNotification.ETag,
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
                "Skipping {ReferenceNumber} because new notification of the same status {Status} is older: {NewTime} < {OldTime}",
                newNotification.ReferenceNumber,
                newNotification.Status,
                newNotification.UpdatedSource,
                existingNotification.UpdatedSource
            );
            return false;
        }

        if (IsLaterInTheLifecycle(newNotification, existingNotification))
            return true;

        logger.LogInformation(
            "Skipping {ReferenceNumber} because new notification is going backwards: {NewStatus} < {OldStatus} or {NewTime} < {OldTime}",
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
        return notification.Status == ImportNotificationStatus.Amend
            || notification.Status == ImportNotificationStatus.Draft
            || notification.ReferenceNumber.StartsWith("DRAFT", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsLaterInTheLifecycle(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        var newPriority = s_statusPriority.GetValueOrDefault(newNotification.Status!, 0);
        var oldPriority = s_statusPriority.GetValueOrDefault(existingNotification.Status!, 0);

        return newPriority >= oldPriority;
    }
}
