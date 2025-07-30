using System.Collections.Frozen;
using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host.AzureServiceBus;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;

namespace Defra.TradeImportsProcessor.Processor.Consumers;

public class NotificationConsumer(ILogger<NotificationConsumer> logger, ITradeImportsDataApiClient api)
    : IConsumer<JsonElement>,
        IConsumerWithContext
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

    public IConsumerContext? Context { get; set; }
    private string? MessageId => Context?.GetTransportMessage().MessageId;

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
            logger.LogInformation("Skipping {ReferenceNumber} due to status", newNotification.ReferenceNumber);

            return;
        }

        var dataApiImportPreNotification = (DataApiIpaffs.ImportPreNotification)newNotification;

        var existingNotification = await api.GetImportPreNotification(
            newNotification.ReferenceNumber,
            cancellationToken
        );

        if (existingNotification != null && !ShouldProcess(dataApiImportPreNotification, existingNotification))
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
            cancellationToken,
            MessageId
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
            cancellationToken,
            MessageId
        );
    }

    private bool ShouldProcess(
        DataApiIpaffs.ImportPreNotification newNotification,
        ImportPreNotificationResponse existingNotificationResponse
    )
    {
        var existingNotification = existingNotificationResponse.ImportPreNotification;

        if (MessageId is not null && MessageId.Equals(existingNotificationResponse.RequestId))
        {
            // Assumption:
            //  if the data returned from the data API has a request ID matching the current message ID
            //  then the data originated from this message, and it should be updated as such
            //
            // Outstanding questions:
            //  1. if the data was newly created first time around, it would be updated second time
            //     around as it's there on the second attempt - what should happen?
            //          - the changeset generated within the data API would be different
            //              - first time would create
            //              - second time would be no differences for update as it's the same data
            //              - does anything use changeset?
            return true;
        }

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
