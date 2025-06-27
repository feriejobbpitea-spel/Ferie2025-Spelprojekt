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
    public Button resetButton;  // Här är knappen för att resetta keybinds

    private List<string> actions = new List<string> { "Jump", "Sprint", "Shoot", "Left", "Right", "SkipCutscene", "NextSlide" };
    private bool isWaitingForKey = false;
    private string currentAction;

    private Dictionary<string, KeyCode> tempBindings = new Dictionary<string, KeyCode>();

    // Standardbindningar
    private Dictionary<string, KeyCode> defaultBindings = new Dictionary<string, KeyCode>
    {
        { "Jump", KeyCode.Space },
        { "Sprint", KeyCode.LeftShift },
        { "Shoot", KeyCode.Mouse0 },
        { "Left", KeyCode.A },
        { "Right", KeyCode.D },
        { "SkipCutscene", KeyCode.Backspace },     // Ändra vid behov
        { "NextSlide", KeyCode.Return }         // Ändra vid behov
    };

    void Start()
    {
        actionsDropdown.ClearOptions();
        actionsDropdown.AddOptions(actions);

        // Initiera bindningar från PlayerPrefs eller default
        foreach (var action in actions)
        {
            string savedKey = PlayerPrefs.GetString("bind_" + action, "");
            if (Enum.TryParse<KeyCode>(savedKey, out var key))
            {
                tempBindings[action] = key;
            }
            else if (defaultBindings.ContainsKey(action))
            {
                tempBindings[action] = defaultBindings[action];
            }
            else
            {
                tempBindings[action] = KeyCode.None;
            }
        }

        currentAction = actions[0];
        UpdateRebindButtonText();

        actionsDropdown.onValueChanged.AddListener(OnDropdownChanged);
        rebindButton.onClick.AddListener(OnRebindButtonClicked);
        applyButton.onClick.AddListener(OnApplyClicked);
        backButton.onClick.AddListener(OnBackClicked);

        resetButton.onClick.AddListener(OnResetClicked);  // Lägg till denna rad för reset-knappen
    }

    void OnDropdownChanged(int index)
    {
        if (isWaitingForKey) return;
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
        foreach (var kvp in tempBindings)
        {
            PlayerPrefs.SetString("bind_" + kvp.Key, kvp.Value.ToString());
        }
        PlayerPrefs.Save();
        Debug.Log("Bindings saved!");
    }

    void OnBackClicked()
    {
        foreach (var action in actions)
        {
            string savedKey = PlayerPrefs.GetString("bind_" + action, "");
            if (Enum.TryParse<KeyCode>(savedKey, out var key))
            {
                tempBindings[action] = key;
            }
            else if (defaultBindings.ContainsKey(action))
            {
                tempBindings[action] = defaultBindings[action];
            }
            else
            {
                tempBindings[action] = KeyCode.None;
            }
        }
        UpdateRebindButtonText();
        Debug.Log("Changes discarded");
    }

    void OnResetClicked()
    {
        // Sätt tempBindings till defaultbindningar
        foreach (var kvp in defaultBindings)
        {
            tempBindings[kvp.Key] = kvp.Value;
        }
        UpdateRebindButtonText();
        Debug.Log("Keybinds reset to default!");
    }
}
