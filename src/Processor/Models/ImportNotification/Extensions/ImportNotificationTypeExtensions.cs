using System;

namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Extensions;

public static class ImportNotificationTypeExtensions
{
    public static string AsString(this ImportNotificationType chedType)
    {
        return chedType switch
        {
            ImportNotificationType.Cveda => "CHEDA",
            ImportNotificationType.Cvedp => "CHEDP",
            ImportNotificationType.Chedpp => "CHEDPP",
            ImportNotificationType.Ced => "CHEDD",
            ImportNotificationType.Imp => "IMP",
            _ => throw new ArgumentOutOfRangeException(nameof(chedType), chedType, null),
        };
    }
}
