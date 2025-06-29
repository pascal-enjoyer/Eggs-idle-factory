using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Image soundIcon;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    [SerializeField] private AudioMixer audioMixer;

    private IAudioService audioService;
    private const float VOLUME_THRESHOLD = 0.01f;

    private void Awake()
    {
        audioService = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        InitializeSliders();
        UpdateIcons();
    }

    private void InitializeSliders()
    {
        if (soundSlider != null)
        {
            soundSlider.value = audioService.GetSoundVolume();
            soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.value = audioService.GetMusicVolume();
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
    }

    private void OnSoundSliderChanged(float value)
    {
        if (value <= VOLUME_THRESHOLD)
        {
            audioMixer.SetFloat("SoundVolume", -80f);
        }
        else
        {
            audioService.SetSoundVolume(value);
        }
        UpdateSoundIcon(value);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (value <= VOLUME_THRESHOLD)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
        }
        else
        {
            audioService.SetMusicVolume(value);
        }
        UpdateMusicIcon(value);
    }

    private void UpdateIcons()
    {
        UpdateSoundIcon(audioService.GetSoundVolume());
        UpdateMusicIcon(audioService.GetMusicVolume());
    }

    private void UpdateSoundIcon(float volume)
    {
        if (soundIcon != null)
        {
            soundIcon.sprite = volume > VOLUME_THRESHOLD ? soundOnSprite : soundOffSprite;
        }
    }

    private void UpdateMusicIcon(float volume)
    {
        if (musicIcon != null)
        {
            musicIcon.sprite = volume > VOLUME_THRESHOLD ? musicOnSprite : musicOffSprite;
        }
    }

    private void OnDestroy()
    {
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveListener(OnSoundSliderChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        }
    }
}