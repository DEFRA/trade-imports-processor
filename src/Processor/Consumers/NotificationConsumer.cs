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
        { ImportNotificationStatus.Amend, 1 },
        { ImportNotificationStatus.Submitted, 2 },
        { ImportNotificationStatus.InProgress, 3 },
        { ImportNotificationStatus.Cancelled, 4 },
        { ImportNotificationStatus.PartiallyRejected, 4 },
        { ImportNotificationStatus.Rejected, 4 },
        { ImportNotificationStatus.Validated, 4 },
        { ImportNotificationStatus.SplitConsignment, 5 },
        { ImportNotificationStatus.Replaced, 5 },
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

        if (ShouldNotProcess(newNotification))
        {
            logger.LogInformation("Skipping {ReferenceNumber} due to status", newNotification.ReferenceNumber);
            return;
        }

        var dataApiImportPreNotification = (DataApiIpaffs.ImportPreNotification)newNotification;

        var existingNotification = await api.GetImportPreNotification(
            newNotification.ReferenceNumber,
            cancellationToken
        );

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

        if (
            NewNotificationIsOlderThanExistingNotification(
                dataApiImportPreNotification,
                existingNotification.ImportPreNotification
            )
        )
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} because new notification is going back in time: {NewTime} < {OldTime}",
                newNotification.ReferenceNumber,
                dataApiImportPreNotification.UpdatedSource,
                existingNotification.ImportPreNotification.UpdatedSource
            );
            return;
        }

        if (!IsLaterInTheLifecycle(dataApiImportPreNotification, existingNotification.ImportPreNotification))
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} because new notification is going back progress status: {NewStatus} < {OldStatus}",
                newNotification.ReferenceNumber,
                dataApiImportPreNotification.Status,
                existingNotification.ImportPreNotification.Status
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

    private static bool NewNotificationIsOlderThanExistingNotification(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        return newNotification.UpdatedSource.TrimMicroseconds() < existingNotification.UpdatedSource.TrimMicroseconds();
    }

    private static bool ShouldNotProcess(ImportNotification notification)
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
