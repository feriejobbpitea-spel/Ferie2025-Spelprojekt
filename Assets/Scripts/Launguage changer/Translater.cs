using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Translater : MonoBehaviour
{
    private LocalizedString localizedString;

    void Start()
    {
        localizedString = new LocalizedString();

        // Ange din tabell (table) och text-nyckel (entry) här
        localizedString.TableReference = "MyTable";        // Ditt tabellnamn i localization
        localizedString.TableEntryReference = "MyEntry";   // Ditt entry-namn i tabellen

        StartCoroutine(WaitForLocalizationAndLoad());
    }

    private System.Collections.IEnumerator WaitForLocalizationAndLoad()
    {
        yield return LocalizationSettings.InitializationOperation;

        var handle = localizedString.GetLocalizedStringAsync();

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Localized text: " + handle.Result);
            // T.ex. sätt text i UI här, t.ex.:
            // myTextComponent.text = handle.Result;
        }
        else
        {
            Debug.LogError("Failed to load localized string.");
        }
    }
}
