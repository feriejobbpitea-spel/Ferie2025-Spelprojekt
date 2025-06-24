using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Dra in din dropdown i Inspector

    private Locale appliedLanguage;  // Språket som är aktivt (det senaste "Apply"-språket)
    private Locale pendingLanguage;  // Språket användaren har valt men inte applicerat än

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

        // Initiera appliedLanguage och pendingLanguage med det språk som är aktivt nu
        appliedLanguage = LocalizationSettings.SelectedLocale;
        pendingLanguage = appliedLanguage;

        // Sätt dropdown till appliedLanguage
        int currentIndex = locales.IndexOf(appliedLanguage);
        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void OnLanguageChanged(int index)
    {
        // Spara pending språk men byt inte språk direkt
        pendingLanguage = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void ApplyLanguageChange()
    {
        if (pendingLanguage != null && LocalizationSettings.SelectedLocale != pendingLanguage)
        {
            LocalizationSettings.SelectedLocale = pendingLanguage;
            appliedLanguage = pendingLanguage;  // Uppdatera appliedLanguage
            Debug.Log("Språk ändrat till: " + appliedLanguage.LocaleName);
        }
    }

    // Den här metoden kan du kalla när du "stänger" settings utan att trycka Apply,
    // för att återställa dropdown till appliedLanguage
    public void ResetDropdownToApplied()
    {
        if (appliedLanguage != null)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            int index = locales.IndexOf(appliedLanguage);
            dropdown.SetValueWithoutNotify(index);
            dropdown.RefreshShownValue();

            // Även uppdatera pendingLanguage så det matchar appliedLanguage
            pendingLanguage = appliedLanguage;
        }
    }
}
