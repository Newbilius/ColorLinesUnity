using System;
using System.Linq;

public static class Config
{
    private const string PathValueKey = "Path";
    private const string SoundSystemValueKey = "SoundSystem";
    private const string NextColorsValueKey = "NextColors";
    private const string ScoreValueKey = "Scores";

    private static int maximumSoundSystemValue = Enum.GetValues(typeof(SoundSystemConfig)).Cast<int>().Max();

    public static bool GetPathValue()
    {
        return PlayerPrefsHelper.GetBoolFromConfig(PathValueKey, true);
    }

    public static bool GetNextColorsValue()
    {
        return PlayerPrefsHelper.GetBoolFromConfig(NextColorsValueKey, true);
    }

    public static SoundSystemConfig GetSoundSystemMode()
    {
        return PlayerPrefsHelper.GetEnum(SoundSystemValueKey, SoundSystemConfig.MusicAndSound);
    }

    public static void SwitchSoundSystemMode()
    {
        var currentValue = PlayerPrefsHelper.GetEnum(SoundSystemValueKey, SoundSystemConfig.MusicAndSound);
        currentValue++;
        if ((int)currentValue > maximumSoundSystemValue)
            currentValue = SoundSystemConfig.MusicAndSound;
        PlayerPrefsHelper.SaveEnum(SoundSystemValueKey,
            currentValue);
    }

    public static bool SwitchPath()
    {
        return PlayerPrefsHelper.SwitchBooleanValue(PathValueKey);
    }

    public static void SwitchNextColors()
    {
        PlayerPrefsHelper.SwitchBooleanValue(NextColorsValueKey);
    }

    public static Scores GetRawScores()
    {
        var results = PlayerPrefsHelper.GetJson<Scores>(ScoreValueKey);
        if (results == null)
            results = new Scores();
        return results;
    }

    public static void SaveScores(Scores scores)
    {
        PlayerPrefsHelper.SaveJson(ScoreValueKey, scores);
    }
}