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
        // Draft
        (ImportNotificationStatus.Draft, ImportNotificationStatus.Draft),
        (ImportNotificationStatus.Draft, ImportNotificationStatus.Deleted),
        (ImportNotificationStatus.Draft, ImportNotificationStatus.Submitted),
        // Amend
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Deleted),
        (ImportNotificationStatus.Amend, ImportNotificationStatus.Submitted),
        // Submitted
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Submitted),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.InProgress),
        (ImportNotificationStatus.Submitted, ImportNotificationStatus.Validated), // auto clearance process
        // In progress
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.InProgress),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Amend),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Validated),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Cancelled),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Rejected),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Replaced),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.Modify),
        (ImportNotificationStatus.InProgress, ImportNotificationStatus.PartiallyRejected),
        // Modify
        (ImportNotificationStatus.Modify, ImportNotificationStatus.Modify),
        (ImportNotificationStatus.Modify, ImportNotificationStatus.InProgress),
        // Partially rejected
        (ImportNotificationStatus.PartiallyRejected, ImportNotificationStatus.PartiallyRejected),
        (ImportNotificationStatus.PartiallyRejected, ImportNotificationStatus.SplitConsignment),
    ];

    private static readonly FrozenDictionary<string, int> s_statusPriority = new Dictionary<string, int>
    {
        { ImportNotificationStatus.Draft, 0 },
        { ImportNotificationStatus.Deleted, 0 },
        { ImportNotificationStatus.Amend, 0 },
        { ImportNotificationStatus.Submitted, 0 },
        { ImportNotificationStatus.Modify, 0 },
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

        LogIuuInformation(newNotification);
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
        if (!IsStateTransitionAllowed(newNotification, existingNotification))
        {
            logger.LogWarning(
                "Unexpected IPAFFS State Transition - Previous state [{From}], new state [{To}]",
                existingNotification.Status,
                newNotification.Status
            );
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

    private void LogIuuInformation(ImportNotification importNotification)
    {
        if (importNotification.PartOne?.Commodities?.ComplementParameterSets != null)
        {
            foreach (
                var keyDataPairs in importNotification
                    .PartOne.Commodities.ComplementParameterSets.Where(x => x.KeyDataPairs is not null)
                    .Select(x => x.KeyDataPairs)
            )
            {
                foreach (DataApiIpaffs.KeyDataPair? keyDataPair in keyDataPairs!)
                {
                    if (keyDataPair?.Key == "is_catch_certificate_required")
                    {
                        logger.LogInformation(
                            "{ReferenceNumber} IUU is_catch_certificate_required  {Value} ",
                            importNotification.ReferenceNumber,
                            keyDataPair.Data
                        );
                    }
                }
            }
        }

        if (importNotification.PartTwo?.ControlAuthority?.IuuCheckRequired != null)
        {
            logger.LogInformation(
                "{ReferenceNumber} IUU IuuCheckRequired  {Value} ",
                importNotification.ReferenceNumber,
                importNotification.PartTwo?.ControlAuthority?.IuuCheckRequired
            );
        }
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
            || notification.Status == ImportNotificationStatus.Modify
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

    private static bool IsStateTransitionAllowed(
        DataApiIpaffs.ImportPreNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    ) => s_statusStateMachine.Contains((existingNotification.Status!, newNotification.Status!));
}
