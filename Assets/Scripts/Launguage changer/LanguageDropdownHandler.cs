using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Dra in din dropdown i Inspector
    private const string PlayerPrefKey = "SelectedLanguage";

    private Locale appliedLanguage;  // Spr�ket som �r aktivt (det senaste "Apply"-spr�ket)
    private Locale pendingLanguage;  // Spr�ket anv�ndaren har valt men inte applicerat �n

    void Start()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        dropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var locale in locales)
        {
            options.Add(locale.LocaleName);
        }

        dropdown.AddOptions(options);

        // L�s sparat spr�k fr�n PlayerPrefs
        string savedLocaleName = PlayerPrefs.GetString(PlayerPrefKey, null);

        if (!string.IsNullOrEmpty(savedLocaleName))
        {
            appliedLanguage = locales.Find(l => l.LocaleName == savedLocaleName);
            if (appliedLanguage == null)
                appliedLanguage = LocalizationSettings.SelectedLocale;
        }
        else
        {
            appliedLanguage = LocalizationSettings.SelectedLocale;
        }

        pendingLanguage = appliedLanguage;

        int currentIndex = locales.IndexOf(appliedLanguage);
        if (currentIndex < 0) currentIndex = 0; // fallback
        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);

        // S�tt LocalizationSettings.SelectedLocale till sparat spr�k direkt
        LocalizationSettings.SelectedLocale = appliedLanguage;
    }

    void OnLanguageChanged(int index)
    {
        // Spara pending spr�k men byt inte spr�k direkt
        pendingLanguage = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void ApplyLanguageChange()
    {
        if (pendingLanguage != null && LocalizationSettings.SelectedLocale != pendingLanguage)
        {
            LocalizationSettings.SelectedLocale = pendingLanguage;
            appliedLanguage = pendingLanguage;  // Uppdatera appliedLanguage

            // Spara valt spr�k i PlayerPrefs
            PlayerPrefs.SetString(PlayerPrefKey, appliedLanguage.LocaleName);
            PlayerPrefs.Save();

            Debug.Log("Spr�k �ndrat till: " + appliedLanguage.LocaleName);
        }
    }

    public void ResetDropdownToApplied()
    {
        if (appliedLanguage != null)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            int index = locales.IndexOf(appliedLanguage);
            dropdown.SetValueWithoutNotify(index);
            dropdown.RefreshShownValue();

            pendingLanguage = appliedLanguage;
        }
    }
}