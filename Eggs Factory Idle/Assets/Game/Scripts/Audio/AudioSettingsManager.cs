using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    private static AudioSettingsManager instance;
    private AudioSettings settings;
    private const string SETTINGS_KEY = "AudioSettings";
    [SerializeField] private AudioMixer audioMixer;

    public static AudioSettingsManager Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("AudioSettingsManager");
                instance = go.AddComponent<AudioSettingsManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            settings = instance.settings;
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    private void Start()
    {
        StartCoroutine(ApplyMixerSettingsDelayed());
    }

    public AudioSettings GetSettings() => settings;

    public void SaveSettings()
    {
        if (settings == null)
        {
            settings = new AudioSettings();
        }
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            try
            {
                settings = JsonUtility.FromJson<AudioSettings>(json);
                if (settings == null)
                {
                    settings = new AudioSettings();
                    SaveSettings();
                }
                else
                {
                    settings.masterVolume = Mathf.Clamp01(settings.masterVolume);
                    settings.soundVolume = Mathf.Clamp01(settings.soundVolume);
                    settings.musicVolume = Mathf.Clamp01(settings.musicVolume);
                }
            }
            catch (System.Exception e)
            {
                settings = new AudioSettings();
                SaveSettings();
            }
        }
        else
        {
            settings = new AudioSettings();
            SaveSettings();
        }
    }

    private void ApplyMixerSettings()
    {
        if (audioMixer == null)
        {
            return;
        }

        const float VOLUME_THRESHOLD = 0.01f;
        float masterVolume = settings.masterVolume;
        float soundVolume = settings.soundVolume;
        float musicVolume = settings.musicVolume;

        audioMixer.SetFloat("MasterVolume", masterVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("SoundVolume", soundVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(soundVolume) * 20);
        audioMixer.SetFloat("MusicVolume", musicVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(musicVolume) * 20);
    }

    private System.Collections.IEnumerator ApplyMixerSettingsDelayed()
    {
        yield return new WaitForEndOfFrame();
        ApplyMixerSettings();
    }
}