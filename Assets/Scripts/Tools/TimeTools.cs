using System;
using UnityEngine;

public static class TimeTools
{
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    public static long GetActualTimeStampInSeconds()
        => (long)DateTime.UtcNow.Subtract(_epoch).TotalSeconds;

    public static long GetTimeStampFromDateInSeconds(DateTime date)
    {
        var _elapsedTime = DateTime.SpecifyKind(date, DateTimeKind.Utc) - _epoch;
        return (long)_elapsedTime.TotalSeconds;
    }
    
    public static long ConvertDateInfoInSecondsTimeStamp(DateInfo dateInfo)
    {
        var _dateTime = new DateTime(dateInfo.year, dateInfo.month, dateInfo.day, 
            dateInfo.hour, dateInfo.minute, dateInfo.second, DateTimeKind.Utc);
        return GetTimeStampFromDateInSeconds(_dateTime);
    }

    public static int GetSecondsInMinutes(int minutes)
    {
        var _result = 0;
        while (minutes > 0)
        {
            _result += 60;
            minutes--;
        }
        return _result;
    }

    public static int ConvertSecondsInMinutes(int seconds)
    {
        var _result = 0;
        while (seconds > 60)
        {
            _result++;
            seconds -= 60;
        }
        return _result;
    }

    public static double ConvertSecondsToMilliseconds(long seconds)
    {
        return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
    }
    
    public static long ConvertMillisecondsToSeconds(double milliseconds)
    {
        return (long)TimeSpan.FromMilliseconds(milliseconds).TotalSeconds;
    }
}
