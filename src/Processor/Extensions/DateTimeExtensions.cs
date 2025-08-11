using System;
using System.Collections.Generic;
using System.Linq;

namespace Defra.TradeImportsProcessor.Processor.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimMicroseconds(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, dt.Kind);
    }

    public static DateTime? TrimMicroseconds(this DateTime? dt)
    {
        return dt?.TrimMicroseconds();
    }

    public static DateTime TrimMinutes(this DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, 0, dt.Kind);
    }

    public static DateTime NextHour(this DateTime dt)
    {
        return dt.AddHours(-1).TrimMinutes();
    }

    public static DateTime CurrentHour(this DateTime dt)
    {
        return dt.TrimMinutes();
    }

    public static DateTime Yesterday(this DateTime dt)
    {
        return dt.AddDays(-1);
    }

    public static DateTime Tomorrow(this DateTime dt)
    {
        return dt.AddDays(1);
    }

    public static DateTime WeekAgo(this DateTime dt)
    {
        return dt.AddDays(-7);
    }

    public static DateTime WeekLater(this DateTime dt)
    {
        return dt.AddDays(7);
    }

    public static DateTime MonthAgo(this DateTime dt)
    {
        return dt.AddMonths(-1);
    }

    public static int DaysSinceMonthAgo(this DateTime dt)
    {
        return Convert.ToInt32((dt - dt.AddMonths(-1)).TotalDays);
    }

    public static int DaysUntilMonthLater(this DateTime dt)
    {
        return Convert.ToInt32((dt.AddMonths(1) - dt).TotalDays);
    }

    public static DateTime MonthLater(this DateTime dt)
    {
        return dt.AddMonths(1);
    }

    private static int CreateRandomInt(int min, int max)
    {
        return Random.Shared.Next(min, max);
    }

    public static DateTime RandomTime(this DateTime dt, int maxHour = 23)
    {
        return new DateTime(
            dt.Year,
            dt.Month,
            dt.Day,
            CreateRandomInt(0, maxHour),
            CreateRandomInt(0, 60),
            CreateRandomInt(0, 60),
            dt.Kind
        );
    }

    public static DateOnly ToDate(this DateTime val)
    {
        return DateOnly.FromDateTime(val);
    }

    public static DateOnly ToDate(this DateTime? val)
    {
        return val?.ToDate() ?? DateOnly.MinValue;
    }

    public static TimeOnly ToTime(this DateTime val)
    {
        return TimeOnly.FromDateTime(val);
    }

    /// <summary>
    /// Borrowed from here https://stackoverflow.com/questions/11930565/list-the-months-between-two-dates
    /// </summary>
    /// <param name="endDate"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public static IEnumerable<(int Month, int Year)> MonthsSince(this DateTime endDate, DateTime startDate)
    {
        DateTime iterator;
        DateTime limit;

        if (endDate > startDate)
        {
            iterator = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            limit = endDate;
        }
        else
        {
            iterator = new DateTime(endDate.Year, endDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            limit = startDate;
        }

        while (iterator <= limit)
        {
            yield return (iterator.Month, iterator.Year);

            iterator = iterator.AddMonths(1);
        }
    }

    /// <summary>
    /// Returns
    /// </summary>
    /// <returns>A list of nullable strings as that's what initialise command currently expects</returns>
    public static List<string> RedactedDatasetsSinceNov24()
    {
        var novFirst2024 = new DateTime(2024, 11, 1, 0, 0, 0, DateTimeKind.Utc);

        return DateTime
            .Today.MonthsSince(novFirst2024)
            .Select(((monthYear, i) => $"PRODREDACTED-{monthYear.Year}{monthYear.Month:00}"))
            .ToList<string>();
    }
}
