using System;

public static class ParseEnums
{
    public static T StringToEnum<T>(string value) => (T)Enum.Parse(typeof(T), value, true);
}
