using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RebindKeyButton : MonoBehaviour
{
    public string actionName; // T.ex. "Jump", "Sprint", "TimeSlow"
    public Text keyText;

    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => StartCoroutine(WaitForKey()));
        UpdateKeyLabel();
    }

    IEnumerator WaitForKey()
    {
        keyText.text = "Tryck en knapp...";
        yield return null;

        while (true)
        {
            foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kc))
                {
                    InputSettings.SetKey(actionName + "Key", kc);
                    UpdateKeyLabel();
                    yield break;
                }
            }
            yield return null;
        }
    }

    void UpdateKeyLabel()
    {
        keyText.text = InputSettings.GetKey(actionName + "Key", KeyCode.None).ToString();
    }
}
