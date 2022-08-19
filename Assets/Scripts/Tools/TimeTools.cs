using System;
using UnityEngine;

public static class TimeTools
{
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    
    public static long GetActualTimeStamp()
        => (long)System.DateTime.UtcNow.Subtract(new System.DateTime(
            1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    
    public static long GetTimeStampFromDate(DateTime date)
    {
        var _elapsedTime = date - _epoch;
        return (long)_elapsedTime.TotalSeconds;
    }
    
    public static long ConvertDateInfoInTimeStamp(DateInfo dateInfo)
    {
        var _dateTime = new DateTime(dateInfo.year, dateInfo.month, dateInfo.day, 
            dateInfo.hour, dateInfo.minute, dateInfo.second, DateTimeKind.Utc);
        return GetTimeStampFromDate(_dateTime);
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
}
