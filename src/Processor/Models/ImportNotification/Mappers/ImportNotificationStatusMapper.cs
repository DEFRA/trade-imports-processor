using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImportNotificationStatusEnumMapper
{
    public static IpaffsDataApi.ImportNotificationStatus? Map(ImportNotificationStatus? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            ImportNotificationStatus.Draft => IpaffsDataApi.ImportNotificationStatus.Draft,
            ImportNotificationStatus.Submitted => IpaffsDataApi.ImportNotificationStatus.Submitted,
            ImportNotificationStatus.Validated => IpaffsDataApi.ImportNotificationStatus.Validated,
            ImportNotificationStatus.Rejected => IpaffsDataApi.ImportNotificationStatus.Rejected,
            ImportNotificationStatus.InProgress => IpaffsDataApi.ImportNotificationStatus.InProgress,
            ImportNotificationStatus.Amend => IpaffsDataApi.ImportNotificationStatus.Amend,
            ImportNotificationStatus.Modify => IpaffsDataApi.ImportNotificationStatus.Modify,
            ImportNotificationStatus.Replaced => IpaffsDataApi.ImportNotificationStatus.Replaced,
            ImportNotificationStatus.Cancelled => IpaffsDataApi.ImportNotificationStatus.Cancelled,
            ImportNotificationStatus.Deleted => IpaffsDataApi.ImportNotificationStatus.Deleted,
            ImportNotificationStatus.PartiallyRejected => IpaffsDataApi.ImportNotificationStatus.PartiallyRejected,
            ImportNotificationStatus.SplitConsignment => IpaffsDataApi.ImportNotificationStatus.SplitConsignment,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
