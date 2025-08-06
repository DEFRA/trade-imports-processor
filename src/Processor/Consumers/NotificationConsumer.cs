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
        { ImportNotificationStatus.InProgress, 0 },
        { ImportNotificationStatus.Cancelled, 1 },
        { ImportNotificationStatus.PartiallyRejected, 1 },
        { ImportNotificationStatus.Rejected, 1 },
        { ImportNotificationStatus.Validated, 1 },
        { ImportNotificationStatus.SplitConsignment, 2 },
        { ImportNotificationStatus.Replaced, 2 },
    }.ToFrozenDictionary();

    public async Task OnHandle(JsonElement received, CancellationToken cancellationToken)
    {
        var newNotification =
            received.Deserialize<ImportNotification>()
            ?? throw new InvalidOperationException("Received invalid message, deserialised as null");

        logger.LogInformation("Received notification {ReferenceNumber}", newNotification.ReferenceNumber);

        if (IsDraftStatus(newNotification))
        {
            logger.LogInformation("Skipping {ReferenceNumber} due to draft status", newNotification.ReferenceNumber);
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

        if (IsLaterInTheLifecycle(newNotification, existingNotification))
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

    private static bool IsDraftStatus(ImportNotification notification)
    {
        return notification.Status == ImportNotificationStatus.Draft
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
