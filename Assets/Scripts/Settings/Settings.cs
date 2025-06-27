using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Settings : MonoBehaviour
{
    [Header("UI Elements")]
    public Button fullscreenToggle;
    public TMP_Text fullscreenToggleText;
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Button applyButton;

    public Button resetButton;

    private Resolution[] resolutions;

    // Temporary Settings
    private bool pendingFullscreen;
    private float pendingVolume;
    private int pendingResolutionIndex;

    // PlayerPrefs keys
    private const string FullscreenKey = "Settings.Fullscreen";
    private const string VolumeKey = "Settings.Volume";
    private const string ResolutionKey = "Settings.ResolutionIndex";

    // --- NYTT: Språk ---
    private const string LanguageKey = "Settings.Language";  // PlayerPrefs-nyckel för språk

    void Start()
    {
        LoadResolutions();
        LoadInitialSettings();

        // NYTT: Ladda sparat språk från PlayerPrefs, annars default till engelska
        int savedLocaleIndex = PlayerPrefs.GetInt(LanguageKey, 0);
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (savedLocaleIndex >= 0 && savedLocaleIndex < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[savedLocaleIndex];
        }
    }

    void OnEnable() => AddListeners();
    void OnDisable() => RemoveListeners();

    // === Initialization ===
    private void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int savedIndex = PlayerPrefs.GetInt(ResolutionKey, GetCurrentResolutionIndex());

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = Mathf.Clamp(savedIndex, 0, options.Count - 1);
        resolutionDropdown.RefreshShownValue();

        pendingResolutionIndex = resolutionDropdown.value;
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                return i;
        }
        return 0;
    }

    private void LoadInitialSettings()
    {
        pendingFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        pendingVolume = PlayerPrefs.GetFloat(VolumeKey, AudioListener.volume);

        fullscreenToggleText.text = pendingFullscreen ? "On" : "Off";
        volumeSlider.value = pendingVolume;
    }

    // === Event Handlers ===
    public void ToggleFullscreen()
    {
        pendingFullscreen = !pendingFullscreen;
        fullscreenToggleText.text = pendingFullscreen ? "On" : "Off";
    }

    public void OnVolumeChanged(float value) => pendingVolume = value;
    public void OnResolutionChanged(int index) => pendingResolutionIndex = index;

    public void ApplySettings()
    {
        // Apply to system
        Screen.fullScreen = pendingFullscreen;
        AudioListener.volume = pendingVolume;
        Resolution res = resolutions[pendingResolutionIndex];
        Screen.SetResolution(res.width, res.height, pendingFullscreen);

        // Save to PlayerPrefs
        PlayerPrefs.SetInt(FullscreenKey, pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(VolumeKey, pendingVolume);
        PlayerPrefs.SetInt(ResolutionKey, pendingResolutionIndex);

        // NYTT: Spara valt språk i PlayerPrefs
        int localeIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        PlayerPrefs.SetInt(LanguageKey, localeIndex);

        PlayerPrefs.Save();
    }

    public void ResetSettings()
    {
        // Återställ till standard
        pendingFullscreen = false;      // Fönsterläge av
        pendingVolume = 1.0f;           // Max volym
        pendingResolutionIndex = 0;     // Första upplösningen i listan

        // NYTT: Återställ språk till engelska (index 0 antaget engelska)
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (locales.Count > 0)
        {
            LocalizationSettings.SelectedLocale = locales[0];
            PlayerPrefs.SetInt(LanguageKey, 0);
        }

        // Tillämpa direkt
        Screen.fullScreen = pendingFullscreen;
        AudioListener.volume = pendingVolume;

        if (resolutions.Length > 0)
        {
            Resolution res = resolutions[pendingResolutionIndex];
            Screen.SetResolution(res.width, res.height, pendingFullscreen);
        }

        // Uppdatera UI
        fullscreenToggleText.text = "Off";
        volumeSlider.value = pendingVolume;
        resolutionDropdown.value = pendingResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Spara till PlayerPrefs
        PlayerPrefs.SetInt(FullscreenKey, 0);
        PlayerPrefs.SetFloat(VolumeKey, 1.0f);
        PlayerPrefs.SetInt(ResolutionKey, 0);
        PlayerPrefs.Save();

        Debug.Log("Settings have been reset to default.");
    }

    // === UI Event Binding ===
    private void AddListeners()
    {
        fullscreenToggle?.onClick.AddListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.AddListener(OnVolumeChanged);
        resolutionDropdown?.onValueChanged.AddListener(OnResolutionChanged);
        applyButton?.onClick.AddListener(ApplySettings);

        resetButton?.onClick.AddListener(ResetSettings);  // Här kopplar vi reset-knappen!
    }

    private void RemoveListeners()
    {
        fullscreenToggle?.onClick.RemoveListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.RemoveListener(OnVolumeChanged);
        resolutionDropdown?.onValueChanged.RemoveListener(OnResolutionChanged);
        applyButton?.onClick.RemoveListener(ApplySettings);

        resetButton?.onClick.RemoveListener(ResetSettings);
    }
}
