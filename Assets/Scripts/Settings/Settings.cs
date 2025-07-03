using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class Settings : MonoBehaviour
{
    [Header("UI Elements")]
    public Button fullscreenToggle;
    public TMP_Text fullscreenToggleText;

    public Slider MasterVolumeSlider;
    public Slider SFXVolumeSlider;
    public Slider MusicVolumeSlider;

    public Button applyButton;
    public Button resetButton;

    [Header("Audio")]
    public AudioMixer audioMixer;

    private const string FullscreenKey = "Settings.Fullscreen";
    private const string MasterVolumeKey = "Settings.MasterVolume";
    private const string SFXVolumeKey = "Settings.SfxVolume";
    private const string MusicVolumeKey = "Settings.MusicVolume";
    private const string LanguageKey = "Settings.Language";

    private bool pendingFullscreen;

    private float pendingMasterVolume;
    private float pendingSfxVolume;
    private float pendingMusicVolume;

    private void Start()
    {
        LoadInitialSettings();

        LocalizationSettings.InitializationOperation.Completed += op =>
        {
            int savedLocaleIndex = PlayerPrefs.GetInt(LanguageKey, 0);
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (savedLocaleIndex >= 0 && savedLocaleIndex < locales.Count)
                LocalizationSettings.SelectedLocale = locales[savedLocaleIndex];
        };
    }

    private void LoadInitialSettings()
    {
        pendingFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggleText.text = pendingFullscreen ? "On" : "Off";

        pendingMasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        pendingSfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        pendingMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        MasterVolumeSlider.value = pendingMasterVolume;
        SFXVolumeSlider.value = pendingSfxVolume;
        MusicVolumeSlider.value = pendingMusicVolume;

        UpdateAllVolumes();
    }

    private void UpdateAllVolumes()
    {
        SetMixerVolume("MasterVolume", pendingMasterVolume);
        SetMixerVolume("MusicVolume", pendingMusicVolume);
        SetMixerVolume("SFXVolume", pendingSfxVolume);
    }

    private void SetMixerVolume(string exposedParam, float linearVolume)
    {
        // Konverterar 0.0001–1.0 till -80 dB till 0 dB
        float dB = Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
    }

    public void ToggleFullscreen()
    {
        pendingFullscreen = !pendingFullscreen;
        fullscreenToggleText.text = pendingFullscreen ? "On" : "Off";
    }

    public void OnMasterVolumeChanged(float value)
    {
        pendingMasterVolume = value;
        UpdateAllVolumes();
    }

    public void OnSfxVolumeChanged(float value)
    {
        pendingSfxVolume = value;
        UpdateAllVolumes();
    }

    public void OnMusicVolumeChanged(float value)
    {
        pendingMusicVolume = value;
        UpdateAllVolumes();
    }

    public void ApplySettings()
    {
        Screen.fullScreen = pendingFullscreen;

        PlayerPrefs.SetInt(FullscreenKey, pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(MasterVolumeKey, pendingMasterVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, pendingSfxVolume);
        PlayerPrefs.SetFloat(MusicVolumeKey, pendingMusicVolume);

        int localeIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        PlayerPrefs.SetInt(LanguageKey, localeIndex);

        PlayerPrefs.Save();
    }

    public void ResetSettings()
    {
        pendingFullscreen = false;
        fullscreenToggleText.text = "Off";

        pendingMasterVolume = 1f;
        pendingSfxVolume = 1f;
        pendingMusicVolume = 1f;

        MasterVolumeSlider.value = pendingMasterVolume;
        SFXVolumeSlider.value = pendingSfxVolume;
        MusicVolumeSlider.value = pendingMusicVolume;

        UpdateAllVolumes();

        PlayerPrefs.SetInt(FullscreenKey, 0);
        PlayerPrefs.SetFloat(MasterVolumeKey, 1f);
        PlayerPrefs.SetFloat(SFXVolumeKey, 1f);
        PlayerPrefs.SetFloat(MusicVolumeKey, 1f);

        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (locales.Count > 0)
        {
            LocalizationSettings.SelectedLocale = locales[0];
            PlayerPrefs.SetInt(LanguageKey, 0);
        }

        PlayerPrefs.Save();
    }

    private void OnEnable()
    {
        fullscreenToggle?.onClick.AddListener(ToggleFullscreen);
        MasterVolumeSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
        SFXVolumeSlider?.onValueChanged.AddListener(OnSfxVolumeChanged);
        MusicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        applyButton?.onClick.AddListener(ApplySettings);
        resetButton?.onClick.AddListener(ResetSettings);
    }

    private void OnDisable()
    {
        fullscreenToggle?.onClick.RemoveListener(ToggleFullscreen);
        MasterVolumeSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        SFXVolumeSlider?.onValueChanged.RemoveListener(OnSfxVolumeChanged);
        MusicVolumeSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        applyButton?.onClick.RemoveListener(ApplySettings);
        resetButton?.onClick.RemoveListener(ResetSettings);
    }
}
