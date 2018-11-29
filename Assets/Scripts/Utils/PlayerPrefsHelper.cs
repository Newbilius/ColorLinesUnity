using System;
using UnityEngine;

public static class PlayerPrefsHelper
{
    public static bool GetBoolFromConfig(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, 1).IntToBool();
    }

    public static bool SwitchBooleanValue(string key)
    {
        var currentValue = PlayerPrefs.GetInt(key, 1).IntToBool();
        PlayerPrefs.SetInt(key, (!currentValue).BoolToInt());
        return (!currentValue);
    }

    public static T GetJson<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key)); ;
        return default(T);
    }

    public static void SaveJson<T>(string key, T data)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(data));
    }

    public static void SaveEnum<T>(string key, T value)
        where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            throw new Exception("Не enum :-/");

        PlayerPrefs.SetInt(key, (int)(object)value);
    }

    public static T GetEnum<T>(string key, T defaultValue)
        where T : struct, IConvertible
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;
        if (!typeof(T).IsEnum)
            throw new Exception("Не enum :-/");
        var intValue = PlayerPrefs.GetInt(key);
        return (T)Enum.Parse(typeof(T), intValue.ToString(), true);
    }
}
