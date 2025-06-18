using UnityEngine;
using TMPro;

public class PromptManager : Singleton<PromptManager>
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialTextComponent;

    private bool isShowing = false;
    private KeyCode currentKeyToPress;

    private void Update()
    {
        // Om en skylt visas och rätt knapp trycks
        if (isShowing && Input.GetKeyDown(currentKeyToPress))
        {
            HideTutorial();
        }
    }

    public void ShowTutorial(string text, KeyCode keyToPress)
    {
        if (tutorialPanel != null && tutorialTextComponent != null)
        {
            tutorialPanel.SetActive(true);
            tutorialTextComponent.text = text;

            currentKeyToPress = keyToPress;
            isShowing = true;
        }
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            isShowing = false;
        }
    }
}
