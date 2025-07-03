using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string rebindKey;     // Ex: "Jump", "Left", "Sprint"
    public string actionName;
    public string tutorialMessageTemplate = "Press {0} to {1}"; // Ex: "Press A to jump"

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;

            KeyCode key = GetBoundKeyForAction(rebindKey);
            string keyName = KeyCodeToString(key);
            string message = string.Format(tutorialMessageTemplate, keyName, actionName.ToLower());

            PromptManager.Instance.ShowTutorial(message, key);
        }
    }

    private KeyCode GetBoundKeyForAction(string action)
    {
        string keyString = PlayerPrefs.GetString("bind_" + action, null);
        Debug.Log($"GetBoundKeyForAction - action: {action}, PlayerPrefs keyString: '{keyString}'");

        // Om PlayerPrefs saknar key eller keyString är ogiltigt, fallbacka till default
        if (string.IsNullOrEmpty(keyString) || !System.Enum.IsDefined(typeof(KeyCode), keyString))
        {
            string defaultKey = GetDefaultKeyForAction(action);
            if (System.Enum.TryParse<KeyCode>(defaultKey, out var defaultKeyCode))
            {
                Debug.Log($"Fallback to default key: {defaultKeyCode} for action {action}");
                return defaultKeyCode;
            }
            Debug.LogWarning($"No valid key found for action {action}, returning KeyCode.None");
            return KeyCode.None;
        }

        // Försök parsa PlayerPrefs värdet till KeyCode
        if (System.Enum.TryParse<KeyCode>(keyString, out var key))
        {
            return key;
        }

        Debug.LogWarning($"Failed to parse KeyCode from '{keyString}' for action '{action}', returning KeyCode.None");
        return KeyCode.None;
    }

    private string GetDefaultKeyForAction(string action)
    {
        return action switch
        {
            "Left" => "A",
            "Right" => "D",
            "Jump" => "Space",
            "Sprint" => "LeftShift",
            "Mouse0" => "Mouse0",  // Viktigt att Unity KeyCode känner igen detta
            _ => ""
        };
    }

    private string KeyCodeToString(KeyCode key)
    {
        Debug.Log($"KeyCodeToString - key: {key}");

        if (key == KeyCode.Space) return "SPACE";
        if (key == KeyCode.LeftShift) return "LEFT SHIFT";
        if (key == KeyCode.Mouse0) return "LEFT MOUSE";
        if (key == KeyCode.None) return "UNKNOWN"; // Valfri fallbacktext istället för "NONE"
        return key.ToString().ToUpper();
    }
}
