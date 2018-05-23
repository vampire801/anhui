using UnityEngine;
using System.Collections;
using System;

public class Misc
{
    public static string FormatTime(uint t)
    {
        DateTime time = new DateTime(1970, 1, 1);
        time = time.AddSeconds(t);
        string sTime = time.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

        return sTime;
    }

    public static string FormatTime_Date(uint t)
    {
        DateTime time = new DateTime(1970, 1, 1);
        time = time.AddSeconds(t);
        string sTime = time.ToLocalTime().ToString(("yyyy/MM/dd"), System.Globalization.DateTimeFormatInfo.InvariantInfo);

        return sTime;
    }

    public static string FormatTime_Time(uint t)
    {
        DateTime time = new DateTime(1970, 1, 1);
        time = time.AddSeconds(t);
        string sTime = time.ToLocalTime().ToString("HH:mm:ss");

        return sTime;
    }
}
