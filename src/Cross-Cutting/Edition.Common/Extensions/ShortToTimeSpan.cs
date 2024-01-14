namespace Edition.Common.Extensions;

public static class ShortToTimeSpan
{
    /// <summary>
    /// Return Time span
    /// </summary>
    /// <param name="minute">duration based on minute</param>
    /// <returns><see cref="TimeSpan"/></returns>
    public static TimeSpan ToTimeSpan(this short minute) => new(0, minute, 0);
    /// <summary>
    /// Return nullable Time span
    /// </summary>
    /// <param name="minute">duration based on minute</param>
    /// <returns><see cref="TimeSpan?"/></returns>
    public static TimeSpan? ToTimeSpan(this short? minute)
    {
        return minute?.ToTimeSpan();
    }
}