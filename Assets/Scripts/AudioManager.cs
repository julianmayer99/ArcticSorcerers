using Assets.Scripts;
using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public ArcticSound[] sounds;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (ArcticSound s in sounds)
        {
            if (s.autoLoadToMemory)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.mixerGroup;
            }
        }
    }

    void LoadSoundSource(ArcticSound s)
    {
        if (s.source != null)
            return;

        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
        s.source.outputAudioMixerGroup = s.mixerGroup;
    }

    void UnloadSoundSource(ArcticSound s)
    {
        if (s.source == null)
            return;

        Destroy(s.source);
        s.source = null;
    }

    public void Play(string name)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("The sound " + name + " has not been found.");
            return;
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        if (s.source == null)
        {
            Debug.LogError("The sound " + name + " could not be loaded.");
            return;
        }

        s.source.Play();
    }

    public void Play(string name, bool onlyPlayIfNotAlreadyPlaying)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("The sound " + name + " has not been found.");
            return;
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }

        if (onlyPlayIfNotAlreadyPlaying && !s.source.isPlaying)
            s.source.Play();
    }

    public void Play(string name, float timeDelay)
    {
        StartCoroutine(PlayAfterTime(name, timeDelay));
    }

    IEnumerator PlayAfterTime(string sound, float timeDelay)
    {
        yield return new WaitForSecondsRealtime(timeDelay);
        Play(sound);
    }

    public void Stop(string name)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
            return;
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        s.source.Stop();
    }

    public void Pause(string name)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
            return;
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        s.source.Pause();
    }

    public void UnPause(string name)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
            return;
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        s.source.UnPause();
    }

    public void FadeOut(string name, float time)
    {
        StartCoroutine(FadeSoundOut(name, time));
    }

    IEnumerator FadeSoundOut(string name, float time)
    {
        Debug.Log("Fading out sound " + name + " in " + time);

        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        for (int i = 0; i < time * 25; i++)
        {
            s.source.volume -= i / (time * 25);
            yield return new WaitForSecondsRealtime(.04f);
        }
        s.source.Stop();

        Debug.Log("Sound " + name + " faded out");
        s.source.volume = s.volume;
    }

    public void PitchShiftDown(string name, float downTo, float transitionDuration)
    {
        StartCoroutine(PitchDown(name, downTo, transitionDuration));
    }

    IEnumerator PitchDown(string name, float downTo, float transitionDuration)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        for (int i = 0; i < transitionDuration * 25; i++)
        {
            s.source.pitch = Mathf.Lerp(s.pitch, downTo, i / transitionDuration * 25);
            yield return new WaitForSecondsRealtime(.04f);
        }
    }

    public void PitchShiftBackUp(string name, float transitionDuration)
    {
        StartCoroutine(PitchUp(name, transitionDuration));
    }

    IEnumerator PitchUp(string name, float transitionDuration)
    {
        ArcticSound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Der Sound " + name + " wurde im AudioManager nicht gefunden.");
        }
        if (s.source == null)
        {
            Debug.Log("The sound " + name + " will be dynamically loaded.");
            LoadSoundSource(s);
        }
        float startPitchLow = s.source.pitch;
        for (int i = 0; i < transitionDuration * 25; i++)
        {
            s.source.pitch = Mathf.Lerp(startPitchLow, s.pitch, i / transitionDuration * 25);
            yield return new WaitForSecondsRealtime(.04f);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        try
        {
            ArcticSound[] s = Array.FindAll(sounds, sound => sound.stopOnSceneChance);
            foreach (var sound in s)
            {
                sound.source.Stop();
            }
        }
        catch (Exception)
        { Debug.LogWarning("Source beim Audiomanager war null."); }
        
        ArcticSound[] s2 = Array.FindAll(sounds, sound => !sound.autoLoadToMemory);
        foreach (var sound in s2)
        {
            UnloadSoundSource(sound);
        }

    }

    public void PlayLevelScore()
    {
        switch (GameSettings.selectedMap)
        {
            case GameSettings.Map.IceRocks: Play("music_level_iceRocks"); break;
            case GameSettings.Map.SnowCastle: Play("music_level_snowCastle"); break;
            default: Play("music_level_iceRocks"); break;
        }
    }

    public void PauseLevelScore()
    {
        switch (GameSettings.selectedMap)
        {
            case GameSettings.Map.IceRocks: Pause("music_level_iceRocks"); break;
            case GameSettings.Map.SnowCastle: Pause("music_level_snowCastle"); break;
            default: Play("music_level_iceRocks"); break;
        }
    }
    public void UnPauseLevelScore()
    {
        switch (GameSettings.selectedMap)
        {
            case GameSettings.Map.IceRocks: UnPause("music_level_iceRocks"); break;
            case GameSettings.Map.SnowCastle: UnPause("music_level_snowCastle"); break;
            default: Play("music_level_iceRocks"); break;
        }
    }

    public static string InGameMusicTheme
    {
        get
        {
            switch (GameSettings.selectedMap)
            {
                case GameSettings.Map.IceRocks: return "music_level_iceRocks";
                case GameSettings.Map.SnowCastle: return "music_level_snowCastle";
                default: return "music_level_iceRocks";
            }
        }
    }

    public static string InGameMusicThemeWfs
    {
        get
        {
            return InGameMusicTheme + "_wfs";
        }
    }

}
