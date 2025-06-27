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
                Debug.Log("AudioSettingsManager: Created new singleton");
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
            Debug.Log("AudioSettingsManager: Destroyed duplicate instance");
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
            Debug.LogWarning("AudioSettingsManager: Settings is null, initializing new settings");
            settings = new AudioSettings();
        }
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();
        Debug.Log($"AudioSettingsManager: Saved settings: {json}");
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            Debug.Log($"AudioSettingsManager: Loaded JSON: {json}");
            try
            {
                settings = JsonUtility.FromJson<AudioSettings>(json);
                if (settings == null)
                {
                    Debug.LogWarning("AudioSettingsManager: Failed to deserialize settings, using defaults");
                    settings = new AudioSettings();
                    SaveSettings();
                }
                else
                {
                    // Нормализуем значения
                    settings.masterVolume = Mathf.Clamp01(settings.masterVolume);
                    settings.soundVolume = Mathf.Clamp01(settings.soundVolume);
                    settings.musicVolume = Mathf.Clamp01(settings.musicVolume);
                    Debug.Log($"AudioSettingsManager: Loaded settings: master={settings.masterVolume}, sound={settings.soundVolume}, music={settings.musicVolume}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AudioSettingsManager: Error deserializing settings: {e.Message}, using defaults");
                settings = new AudioSettings();
                SaveSettings();
            }
        }
        else
        {
            Debug.Log("AudioSettingsManager: No saved settings found, using defaults");
            settings = new AudioSettings();
            SaveSettings();
        }
    }

    private void ApplyMixerSettings()
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioSettingsManager: AudioMixer not assigned!");
            return;
        }

        const float VOLUME_THRESHOLD = 0.01f;
        float masterVolume = settings.masterVolume;
        float soundVolume = settings.soundVolume;
        float musicVolume = settings.musicVolume;

        audioMixer.SetFloat("MasterVolume", masterVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(masterVolume) * 20);
        audioMixer.SetFloat("SoundVolume", soundVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(soundVolume) * 20);
        audioMixer.SetFloat("MusicVolume", musicVolume <= VOLUME_THRESHOLD ? -80f : Mathf.Log10(musicVolume) * 20);
        Debug.Log($"AudioSettingsManager: Applied mixer settings: master={masterVolume}, sound={soundVolume}, music={musicVolume}");
    }

    private System.Collections.IEnumerator ApplyMixerSettingsDelayed()
    {
        yield return new WaitForEndOfFrame();
        ApplyMixerSettings();
    }
}