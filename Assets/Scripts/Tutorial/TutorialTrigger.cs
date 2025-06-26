using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string actionName;     // Ex: "Jump", "Left", "Sprint"
    public string tutorialMessageTemplate = "Press {0} to {1}"; // Ex: "Press A to jump"

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;

            KeyCode key = GetBoundKeyForAction(actionName);
            string keyName = KeyCodeToString(key);
            string message = string.Format(tutorialMessageTemplate, keyName, actionName.ToLower());

            PromptManager.Instance.ShowTutorial(message, key);
        }
    }

    private KeyCode GetBoundKeyForAction(string action)
    {
        string keyString = PlayerPrefs.GetString("bind_" + action, GetDefaultKeyForAction(action));
        if (System.Enum.TryParse<KeyCode>(keyString, out var key))
        {
            return key;
        }
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
            _ => "None"
        };
    }

    private string KeyCodeToString(KeyCode key)
    {
        if (key == KeyCode.Space) return "SPACE";
        if (key == KeyCode.LeftShift) return "LEFT SHIFT";
        if (key == KeyCode.Mouse0) return "LEFT MOUSE";
        return key.ToString().ToUpper();
    }
}
