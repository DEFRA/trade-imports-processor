using System.Text.Json;
using Defra.TradeImportsDataApi.Api.Client;
using Defra.TradeImportsDataApi.Domain.Ipaffs.Constants;
using Defra.TradeImportsProcessor.Processor.Extensions;
using SlimMessageBus;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;
using ImportNotification = Defra.TradeImportsProcessor.Processor.Models.ImportNotification.ImportNotification;

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
            NewNotificationIsOlderThanExistingNotification(newNotification, existingNotification.ImportPreNotification)
            || GoingBackIntoInProgress(newNotification, existingNotification.ImportPreNotification)
        )
        {
            logger.LogInformation(
                "Skipping {ReferenceNumber} because new notification is going back in time/progress",
                newNotification.ReferenceNumber
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
        ImportNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        return newNotification.LastUpdated.TrimMicroseconds() < existingNotification.UpdatedSource.TrimMicroseconds();
    }

    private static bool ShouldNotProcess(ImportNotification notification)
    {
        return ImportNotificationStatus.IsAmend(notification.Status)
            || ImportNotificationStatus.IsDraft(notification.Status)
            || notification.ReferenceNumber.StartsWith("DRAFT", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool GoingBackIntoInProgress(
        ImportNotification newNotification,
        DataApiIpaffs.ImportPreNotification existingNotification
    )
    {
        return ImportNotificationStatus.IsInProgress(newNotification.Status)
            && (
                existingNotification.StatusIsValidated()
                || existingNotification.StatusIsRejected()
                || existingNotification.StatusIsPartiallyRejected()
            );
    }
}
