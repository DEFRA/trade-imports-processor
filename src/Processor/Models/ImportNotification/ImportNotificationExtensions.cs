using Defra.TradeImportsDataApi.Domain.Ipaffs.Constants;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification;

public static class ImportNotificationExtensions
{
    public static bool StatusIsDraft(this ImportNotification notification) =>
        ImportNotificationStatus.IsDraft(notification.Status);

    public static bool StatusIsAmend(this ImportNotification notification) =>
        ImportNotificationStatus.IsAmend(notification.Status);

    public static bool StatusIsInProgress(this ImportNotification notification) =>
        ImportNotificationStatus.IsInProgress(notification.Status);
}
