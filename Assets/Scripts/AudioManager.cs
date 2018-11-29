using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;
    public AudioSource winMusicAudioSource;

    private void Awake()
    {
        Instance = this;
        ReloadStatesFromConfig();
        musicAudioSource.Play();
    }

    public void PlaySound(AudioClip audioClip)
    {
        soundAudioSource.clip = audioClip;
        soundAudioSource.loop = false;
        soundAudioSource.Play();
    }

    public void PlayLoopSound(AudioClip audioClip)
    {
        soundAudioSource.clip = audioClip;
        soundAudioSource.loop = true;
        soundAudioSource.Play();
    }

    public void StopSound()
    {
        soundAudioSource.Stop();
    }

    private Action onWinMusicComplete;
    private bool winMusicInProgress;

    public void PlayWinMusicIfCan(Action onComplete)
    {
        var curretMusicSetting = Config.GetSoundSystemMode();
        if (curretMusicSetting == SoundSystemConfig.None)
            onComplete();
        else
        {
            onWinMusicComplete = onComplete;
            musicAudioSource.mute = true;
            soundAudioSource.mute = true;
            winMusicAudioSource.Play();
            winMusicInProgress = true;
        }
    }

    void Update()
    {
        if (winMusicInProgress && !winMusicAudioSource.isPlaying)
        {
            winMusicInProgress = false;
            ReloadStatesFromConfig();
            onWinMusicComplete();
            onWinMusicComplete = null;
        }
    }

    public void ReloadStatesFromConfig()
    {
        var newMusicValue = Config.GetSoundSystemMode();
        var oldMusicMute = musicAudioSource.mute;
        switch (newMusicValue)
        {
            case SoundSystemConfig.MusicAndSound:
                musicAudioSource.mute = false;
                soundAudioSource.mute = false;
                break;

            case SoundSystemConfig.Music:
                musicAudioSource.mute = false;
                soundAudioSource.mute = true;
                break;

            case SoundSystemConfig.Sound:
                musicAudioSource.mute = true;
                soundAudioSource.mute = false;
                break;

            case SoundSystemConfig.None:
                musicAudioSource.mute = true;
                soundAudioSource.mute = true;
                break;
        }

        if (!musicAudioSource.mute && oldMusicMute)
            musicAudioSource.Play();
    }
}
