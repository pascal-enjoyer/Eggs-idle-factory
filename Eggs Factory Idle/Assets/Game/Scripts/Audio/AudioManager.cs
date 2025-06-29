using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

public interface IAudioService
{
    void PlaySound(string soundId, Vector3 position, float volume = 1f);
    void PlayMusic(string musicId);
    void StopMusic();
    void SetMasterVolume(float volume);
    void SetSoundVolume(float volume);
    void SetMusicVolume(float volume);
    float GetMasterVolume();
    float GetSoundVolume();
    float GetMusicVolume();
}

[Serializable]
public class AudioSettings
{
    public float masterVolume = 1f;
    public float soundVolume = 1f;
    public float musicVolume = 1f;
}

[Serializable]
public class Sound
{
    public string id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
}

public class AudioManager : MonoBehaviour, IAudioService
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound[] musicTracks;
    [SerializeField] private AudioSource musicSourcePrefab;
    [SerializeField] private AudioSource soundSourcePrefab;

    private Dictionary<string, Sound> soundDict;
    private Dictionary<string, Sound> musicDict;
    private AudioSource currentMusicSource;
    private readonly List<AudioSource> activeSoundSources = new List<AudioSource>();
    private AudioSettingsManager settingsManager;

    private void Awake()
    {
        Debug.Log("AudioManager: Установлен максимальный FPS (targetFrameRate = -1, VSync отключён)");

        InitializeDictionaries();
        settingsManager = AudioSettingsManager.Instance;
        ApplySavedSettings();
    }

    private void Start()
    {
        PlayNextMusic();
    }

    private void InitializeDictionaries()
    {
        soundDict = new Dictionary<string, Sound>();
        musicDict = new Dictionary<string, Sound>();

        foreach (var sound in sounds)
            soundDict.Add(sound.id, sound);

        foreach (var music in musicTracks)
            musicDict.Add(music.id, music);
    }

    private void ApplySavedSettings()
    {
        var settings = settingsManager.GetSettings();
        SetMasterVolume(settings.masterVolume);
        SetSoundVolume(settings.soundVolume);
        SetMusicVolume(settings.musicVolume);
    }

    public void PlaySound(string soundId, Vector3 position, float volume = 1f)
    {
        if (!soundDict.TryGetValue(soundId, out var sound))
        {
            Debug.LogWarning($"Sound with ID {soundId} not found!");
            return;
        }

        var source = Instantiate(soundSourcePrefab, position, Quaternion.identity);
        ConfigureAudioSource(source, sound, volume);
        activeSoundSources.Add(source);

        StartCoroutine(CleanUpSource(source, sound.clip.length / sound.pitch));
    }

    public void PlayMusic(string musicId)
    {
        if (!musicDict.TryGetValue(musicId, out var music))
        {
            Debug.LogWarning($"Music with ID {musicId} not found!");
            return;
        }

        if (currentMusicSource != null)
            Destroy(currentMusicSource.gameObject);

        currentMusicSource = Instantiate(musicSourcePrefab, transform);
        ConfigureAudioSource(currentMusicSource, music, 1f);

        StartCoroutine(WaitForMusicEnd(music.clip.length / music.pitch));
    }

    private void PlayNextMusic()
    {
        string musicId = "0";
        PlayMusic(musicId);
    }

    private System.Collections.IEnumerator WaitForMusicEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayNextMusic();
    }

    public void StopMusic()
    {
        if (currentMusicSource != null)
        {
            Destroy(currentMusicSource.gameObject);
            currentMusicSource = null;
        }
    }

    private void ConfigureAudioSource(AudioSource source, Sound sound, float volumeScale)
    {
        source.clip = sound.clip;
        source.volume = sound.volume * volumeScale;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.Play();
    }

    private System.Collections.IEnumerator CleanUpSource(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (source != null)
        {
            activeSoundSources.Remove(source);
            Destroy(source.gameObject);
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        settingsManager.GetSettings().masterVolume = volume;
        settingsManager.SaveSettings();
    }

    public void SetSoundVolume(float volume)
    {
        audioMixer.SetFloat("SoundVolume", Mathf.Log10(volume) * 20);
        settingsManager.GetSettings().soundVolume = volume;
        settingsManager.SaveSettings();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        settingsManager.GetSettings().musicVolume = volume;
        settingsManager.SaveSettings();
    }

    public float GetMasterVolume() => settingsManager.GetSettings().masterVolume;
    public float GetSoundVolume() => settingsManager.GetSettings().soundVolume;
    public float GetMusicVolume() => settingsManager.GetSettings().musicVolume;
}