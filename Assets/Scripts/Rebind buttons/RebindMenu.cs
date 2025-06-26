using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebindMenu : MonoBehaviour
{
    public TMP_Dropdown actionsDropdown;
    public Button rebindButton;
    public TMP_Text rebindButtonText;

    public Button applyButton;
    public Button backButton;

    // Lista p� alla actions som kan bindas
    private List<string> actions = new List<string>
    {
        "Jump", "Sprint", "Shoot", "Left", "Right", "SkipCutscene", "NextSlide", "Interact"
    };

    private bool isWaitingForKey = false;
    private string currentAction;

    // Tempor�r lagring f�r bindningar som �ndras i menyn
    private Dictionary<string, KeyCode> tempBindings = new Dictionary<string, KeyCode>();

    void Start()
    {
        // Fyll dropdown med actions
        actionsDropdown.ClearOptions();
        actionsDropdown.AddOptions(actions);

        // Initiera tempBindings fr�n PlayerPrefs (eller standard)
        foreach (var action in actions)
        {
            string savedKey = PlayerPrefs.GetString("bind_" + action, GetDefaultKeyForAction(action));
            if (Enum.TryParse<KeyCode>(savedKey, out var key))
            {
                tempBindings[action] = key;
            }
            else
            {
                tempBindings[action] = KeyCode.None;
            }
        }

        // V�lj f�rsta action som default
        currentAction = actions[0];
        UpdateRebindButtonText();

        // Koppla UI-knappar
        actionsDropdown.onValueChanged.AddListener(OnDropdownChanged);
        rebindButton.onClick.AddListener(OnRebindButtonClicked);
        applyButton.onClick.AddListener(OnApplyClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnDropdownChanged(int index)
    {
        if (isWaitingForKey) return; // blocka byte under rebind
        currentAction = actions[index];
        UpdateRebindButtonText();
    }

    void OnRebindButtonClicked()
    {
        if (isWaitingForKey) return;
        isWaitingForKey = true;
        rebindButtonText.text = "Press a key...";
    }

    void Update()
    {
        if (!isWaitingForKey) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    tempBindings[currentAction] = kcode;
                    isWaitingForKey = false;
                    UpdateRebindButtonText();
                    break;
                }
            }
        }
    }

    void UpdateRebindButtonText()
    {
        if (tempBindings.TryGetValue(currentAction, out KeyCode key))
        {
            string keyName = key == KeyCode.None ? "Unbound" : key.ToString();
            rebindButtonText.text = "Current keybind: " + keyName;
        }
        else
        {
            rebindButtonText.text = "Current keybind: Unbound";
        }
    }

    void OnApplyClicked()
    {
        // Spara alla temp-bindningar till PlayerPrefs
        foreach (var kvp in tempBindings)
        {
            PlayerPrefs.SetString("bind_" + kvp.Key, kvp.Value.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("Bindings saved!");
        // Du kan l�gga till kod h�r f�r att st�nga menyn eller visa bekr�ftelse
    }

    void OnBackClicked()
    {
        // �terst�ll tempBindings till sparade v�rden (ignorera �ndringar)
        foreach (var action in actions)
        {
            string savedKey = PlayerPrefs.GetString("bind_" + action, GetDefaultKeyForAction(action));
            if (Enum.TryParse<KeyCode>(savedKey, out var key))
            {
                tempBindings[action] = key;
            }
            else
            {
                tempBindings[action] = KeyCode.None;
            }
        }

        UpdateRebindButtonText();
        Debug.Log("Changes discarded");
        // Du kan l�gga till kod h�r f�r att st�nga menyn
    }

    // H�mtar standardtangent f�r varje action
    private string GetDefaultKeyForAction(string action)
    {
        switch (action)
        {
            case "Jump": return KeyCode.Space.ToString();
            case "Sprint": return KeyCode.LeftShift.ToString();
            case "Shoot": return KeyCode.Mouse0.ToString();
            case "Left": return KeyCode.A.ToString();
            case "Right": return KeyCode.D.ToString();
            case "SkipCutscene": return KeyCode.Return.ToString();
            case "NextSlide": return KeyCode.Return.ToString();
            case "Interact": return KeyCode.E.ToString();
            default: return KeyCode.None.ToString();
        }
    }
}
