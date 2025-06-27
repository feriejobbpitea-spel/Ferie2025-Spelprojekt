using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private Locale appliedLanguage;
    private Locale pendingLanguage;

    void Start()
    {
        StartCoroutine(InitializeDropdown());
    }

    private IEnumerator InitializeDropdown()
    {
        // Wait for localization system to finish initializing
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;

        dropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var locale in locales)
        {
            options.Add(locale.LocaleName);
        }

        dropdown.AddOptions(options);

        appliedLanguage = LocalizationSettings.SelectedLocale;
        pendingLanguage = appliedLanguage;

        int currentIndex = locales.IndexOf(appliedLanguage);
        dropdown.value = currentIndex;
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void OnLanguageChanged(int index)
    {
        pendingLanguage = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void ApplyLanguageChange()
    {
        if (pendingLanguage != null && LocalizationSettings.SelectedLocale != pendingLanguage)
        {
            LocalizationSettings.SelectedLocale = pendingLanguage;
            appliedLanguage = pendingLanguage;
            Debug.Log("Language changed to: " + appliedLanguage.LocaleName);
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
