using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Header("UI Elements")]
    public Button fullscreenToggle;
    public TMP_Text fullscreenToggleText;
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Button applyButton;

    private Resolution[] resolutions;

    // Temporary Settings
    private bool pendingFullscreen;
    private float pendingVolume;
    private int pendingResolutionIndex;

    // PlayerPrefs keys
    private const string FullscreenKey = "Settings.Fullscreen";
    private const string VolumeKey = "Settings.Volume";
    private const string ResolutionKey = "Settings.ResolutionIndex";

    void Start()
    {
        LoadResolutions();
        LoadInitialSettings();
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
        PlayerPrefs.Save();
    }

    // === UI Event Binding ===
    private void AddListeners()
    {
        fullscreenToggle?.onClick.AddListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.AddListener(OnVolumeChanged);
        resolutionDropdown?.onValueChanged.AddListener(OnResolutionChanged);
        applyButton?.onClick.AddListener(ApplySettings);
    }

    private void RemoveListeners()
    {
        fullscreenToggle?.onClick.RemoveListener(ToggleFullscreen);
        volumeSlider?.onValueChanged.RemoveListener(OnVolumeChanged);
        resolutionDropdown?.onValueChanged.RemoveListener(OnResolutionChanged);
        applyButton?.onClick.RemoveListener(ApplySettings);
    }
}
