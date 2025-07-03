using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [Tooltip("Array of input actions to check (e.g., Left, Right, Jump)")]
    public string[] rebindKeys;

    [Tooltip("Localization key for the action (e.g. 'jump', 'move')")]
    public LocalizedString localizedActionName;

    [Tooltip("Localization key for the message template (e.g. 'Press {0} to {1}')")]
    public LocalizedString localizedTemplate;

    [Tooltip("Radius within which the tutorial will be shown")]
    public float triggerRadius = 5f;

    private Transform player;
    private bool hasBeenTriggered = false;

    private void Update()
    {
        if (hasBeenTriggered) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player != null && Vector2.Distance(transform.position, player.position) <= triggerRadius)
        {
            hasBeenTriggered = true;

            List<KeyCode> keys = GetBoundKeysForActions(rebindKeys);
            string keyNames = string.Join(" / ", keys.Select(KeyCodeToString));

            TriggerTutorial(keyNames, keys.ToArray());
        }
    }

    private void TriggerTutorial(string keyNames, KeyCode[] keys)
    {
        var actionLocalized = localizedActionName.GetLocalizedStringAsync().WaitForCompletion();
        var templateLocalized = localizedTemplate.GetLocalizedStringAsync().WaitForCompletion();

        string message = string.Format(templateLocalized, keyNames, actionLocalized.ToLower());

        PromptManager.Instance.ShowTutorial(message, keys);
    }


    private List<KeyCode> GetBoundKeysForActions(string[] actions)
    {
        List<KeyCode> keys = new List<KeyCode>();

        foreach (var action in actions)
        {
            bool actionHasKey = false;

            string keyString = PlayerPrefs.GetString("bind_" + action, null);

            if (!string.IsNullOrEmpty(keyString))
            {
                foreach (var ks in keyString.Split(','))
                {
                    if (System.Enum.TryParse(ks.Trim(), out KeyCode parsedKey) && !keys.Contains(parsedKey))
                    {
                        keys.Add(parsedKey);
                        actionHasKey = true;
                    }
                }
            }

            if (!actionHasKey)
            {
                if (System.Enum.TryParse(GetDefaultKeyForAction(action), out KeyCode defaultKey) && !keys.Contains(defaultKey))
                {
                    keys.Add(defaultKey);
                }
            }
        }

        return keys;
    }


    private string GetDefaultKeyForAction(string action)
    {
        return action switch
        {
            "Left" => "A",
            "Right" => "D",
            "Jump" => "Space",
            "Sprint" => "LeftShift",
            "Shoot" => "Mouse0",
            "Interact" => "E",
            _ => "None"
        };
    }

    private string KeyCodeToString(KeyCode key)
    {
        return key switch
        {
            KeyCode.Space => "SPACE",
            KeyCode.LeftShift => "LEFT SHIFT",
            KeyCode.Mouse0 => "LEFT MOUSE",
            KeyCode.None => "UNKNOWN",
            _ => key.ToString().ToUpper()
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
