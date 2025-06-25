using UnityEngine;

public class AudioSettingsManager : MonoBehaviour
{
    private static AudioSettingsManager instance;
    private AudioSettings settings;
    private const string SETTINGS_KEY = "AudioSettings";

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
            // —охран€ем настройки из существующего экземпл€ра
            settings = instance.settings;
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public AudioSettings GetSettings() => settings;

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            settings = JsonUtility.FromJson<AudioSettings>(json);
        }
        else
        {
            settings = new AudioSettings();
            SaveSettings();
        }
    }
}