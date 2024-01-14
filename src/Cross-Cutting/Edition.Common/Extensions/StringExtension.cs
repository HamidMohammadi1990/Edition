using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Edition.Common.Extensions;

public static class StringExtension
{
    /// <summary>
    /// <para>Initializes a new instance of the <c>System.TimeSpan</c> structure to a specified number
    /// of days, hours, minutes, seconds, and milliseconds.
    /// </para>
    /// </summary>
    /// <param name="value">
    /// value format must be on of this patterns:
    /// <list type="number">
    ///<item>
    /// <description>"Seconds"</description>
    /// </item>
    /// <item>
    /// <description>"Hours:Minutes"</description>
    /// </item>
    /// <item>
    /// <description>"Hours:Minutes:Second"</description>
    /// </item>
    /// <item>
    /// <description>"Days:Hours:Minutes:Second"</description>
    /// </item>
    /// <item>
    /// <description>"Days:Hours:Minutes:Second:Milliseconds"</description>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="separator">Determine a character which split <paramref name="value"/> .Default value is ":"</param>
    /// <returns><c>TimeSpan</c></returns>
    /// <exception cref="ArgumentNullException">The parameter is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">if separated value more than 5 items</exception>
    public static TimeSpan ToTimeSpan(this string value, char separator = ':')
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value == "") throw new FormatException("Input string is empty");
        var timeDetail = value.Split(separator).Select(x =>
        {
            if (int.TryParse(x, out var result))
                return result;
            throw new FormatException("Input string is not numeric");
        }).ToList();
        return timeDetail.Count switch
        {
            1 => TimeSpan.FromSeconds(timeDetail[0]),
            2 => new TimeSpan(timeDetail[0], timeDetail[1], 0),
            3 => new TimeSpan(timeDetail[0], timeDetail[1], timeDetail[2]),
            4 => new TimeSpan(timeDetail[0], timeDetail[1], timeDetail[2], timeDetail[3]),
            5 => new TimeSpan(timeDetail[0], timeDetail[1], timeDetail[2], timeDetail[3], timeDetail[4]),
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
    }
    public static bool HasValue(this string? value, bool ignoreWhiteSpace = true)
    {
        return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
    }

    public static int ToInt(this string value)
    {
        return Convert.ToInt32(value);
    }

    public static decimal ToDecimal(this string value)
    {
        return Convert.ToDecimal(value);
    }

    public static string ToNumeric(this int value)
    {
        return value.ToString("N0"); //"123,456"
    }

    public static string ToNumeric(this decimal value)
    {
        return value.ToString("N0");
    }

    public static string ToCurrency(this int value)
    {
        return value.ToString("C0");
    }

    public static string ToCurrency(this decimal value)
    {
        return value.ToString("C0");
    }

    public static string En2Fa(this string str)
    {
        return str.Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");
    }

    public static string Fa2En(this string str)
    {
        return str.Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            //iphone numeric
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9");
    }

    public static string FixPersianChars(this string str)
    {
        return str.Replace("ﮎ", "ک")
            .Replace("ﮏ", "ک")
            .Replace("ﮐ", "ک")
            .Replace("ﮑ", "ک")
            .Replace("ك", "ک")
            .Replace("ي", "ی")
            .Replace(" ", " ")
            .Replace("‌", " ")
            .Replace("ھ", "ه");
    }

    public static string? CleanString(this string str)
    {
        return str.Trim().FixPersianChars()?.Fa2En().NullIfEmpty();
    }

    public static string? NullIfEmpty(this string str)
    {
        return str?.Length == 0 ? null : str;
    }

    public static string TrimEnd(this string source, string value)
    {
        while (source.EndsWith(value, StringComparison.OrdinalIgnoreCase))
            source = source[..^value.Length];
        return source;
    }

    public static bool IsEmail(this string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsMobile(this string mobile)
    {
        string pattern = @"^(\+98|0098|98|0)?9\d{9}$";
        return Regex.IsMatch(mobile, pattern);
    }
}