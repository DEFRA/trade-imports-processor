namespace Defra.TradeImportsProcessor.Processor.Models.ImportNotification.Mappers;

public static class DateTimeMapper
{
    public static DateTime? Map(DateOnly? date, TimeOnly? time)
    {
        return date?.ToDateTime(time ?? TimeOnly.MinValue);
    }
}
