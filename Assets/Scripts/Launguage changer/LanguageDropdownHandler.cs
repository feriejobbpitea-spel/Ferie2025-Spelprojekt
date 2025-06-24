using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Dra in din dropdown i Inspector

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

        // Initiera appliedLanguage och pendingLanguage med det spr�k som �r aktivt nu
        appliedLanguage = LocalizationSettings.SelectedLocale;
        pendingLanguage = appliedLanguage;

        // S�tt dropdown till appliedLanguage
        int currentIndex = locales.IndexOf(appliedLanguage);
        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);
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
            Debug.Log("Spr�k �ndrat till: " + appliedLanguage.LocaleName);
        }
    }

    // Den h�r metoden kan du kalla n�r du "st�nger" settings utan att trycka Apply,
    // f�r att �terst�lla dropdown till appliedLanguage
    public void ResetDropdownToApplied()
    {
        if (appliedLanguage != null)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            int index = locales.IndexOf(appliedLanguage);
            dropdown.SetValueWithoutNotify(index);
            dropdown.RefreshShownValue();

            // �ven uppdatera pendingLanguage s� det matchar appliedLanguage
            pendingLanguage = appliedLanguage;
        }
    }
}
