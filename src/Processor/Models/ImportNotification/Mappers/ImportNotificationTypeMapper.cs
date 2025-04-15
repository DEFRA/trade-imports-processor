using IpaffsDataApi = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class ImportNotificationTypeEnumMapper
{
    public static IpaffsDataApi.ImportNotificationType? Map(ImportNotificationType? from)
    {
        if (from == null)
            return default!;
        return from switch
        {
            ImportNotificationType.Cveda => IpaffsDataApi.ImportNotificationType.Cveda,
            ImportNotificationType.Cvedp => IpaffsDataApi.ImportNotificationType.Cvedp,
            ImportNotificationType.Chedpp => IpaffsDataApi.ImportNotificationType.Chedpp,
            ImportNotificationType.Ced => IpaffsDataApi.ImportNotificationType.Ced,
            ImportNotificationType.Imp => IpaffsDataApi.ImportNotificationType.Imp,

            _ => throw new ArgumentOutOfRangeException(nameof(from), from, null),
        };
    }
}
