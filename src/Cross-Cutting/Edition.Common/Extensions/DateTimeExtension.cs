namespace Edition.Common.Extensions;

public static class DateTimeExtension
{
    public static DateTime? AddTime(this DateTime? date, string time)
    {
        if (date is null) return null;
        if (string.IsNullOrEmpty(time)) return date;
        TimeSpan timeSpan;
        try
        {
            timeSpan = time.ToTimeSpan();
        }
        catch
        {
            return date;
        }

        return date.Value.AddTicks(timeSpan.Ticks);
    }
}