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
    public Button applyButton;

    public Button resetButton;

    // Temporary Settings
    private bool pendingFullscreen;
    private float pendingVolume;

    // PlayerPrefs keys
    private const string FullscreenKey = "Settings.Fullscreen";
    private const string VolumeKey = "Settings.Volume";

    // --- NYTT: Språk ---
    private const string LanguageKey = "Settings.Language";  // PlayerPrefs-nyckel för språk

    // NYTT: Variabel för att tracka extern fullscreen-ändring
    private bool lastFullscreenState;

    void Start()
    {
        LoadInitialSettings();

        // Initiera lastFullscreenState till nuvarande fullscreen-läge
        lastFullscreenState = Screen.fullScreen;

        // NYTT: Ladda sparat språk från PlayerPrefs, annars default till engelska
        int savedLocaleIndex = PlayerPrefs.GetInt(LanguageKey, 0);
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (savedLocaleIndex >= 0 && savedLocaleIndex < locales.Count)
        {
            LocalizationSettings.SelectedLocale = locales[savedLocaleIndex];
        }
    }

    void Update()
    {
        // Kolla om fullscreen-läget ändrats externt (inte via din knapp)
        if (Screen.fullScreen != lastFullscreenState)
        {
            lastFullscreenState = Screen.fullScreen;
            pendingFullscreen = lastFullscreenState;
            fullscreenToggleText.text = pendingFullscreen ? "On" : "Off";
        }
    }

    void OnEnable() => AddListeners();
    void OnDisable() => RemoveListeners();

    // === Initialization ===

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

    public void ApplySettings()
    {
        // Apply to system
        Screen.fullScreen = pendingFullscreen;
        AudioListener.volume = pendingVolume;

        // Save to PlayerPrefs
        PlayerPrefs.SetInt(FullscreenKey, pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetFloat(VolumeKey, pendingVolume);

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

        // Uppdatera UI
        fullscreenToggleText.text = "Off";
        volumeSlider.value = pendingVolume;

        // Spara till PlayerPrefs
        PlayerPrefs.SetInt(FullscreenKey, 0);
        PlayerPrefs.SetFloat(VolumeKey, 1.0f);
        PlayerPrefs.Save();

        Debug.Log("Settings have been reset to default.");
    }

    // === UI Event Binding ===
    private void AddListeners()
    {
        fullscreenToggle?.onClick.AddListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.AddListener(OnVolumeChanged);
        applyButton?.onClick.AddListener(ApplySettings);

        resetButton?.onClick.AddListener(ResetSettings);  // Här kopplar vi reset-knappen!
    }

    private void RemoveListeners()
    {
        fullscreenToggle?.onClick.RemoveListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.RemoveListener(OnVolumeChanged);
        applyButton?.onClick.RemoveListener(ApplySettings);

        resetButton?.onClick.RemoveListener(ResetSettings);
    }
}
