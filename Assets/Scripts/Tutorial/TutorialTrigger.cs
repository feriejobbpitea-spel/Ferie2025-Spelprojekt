using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string tutorialText;         // Texten som ska visas
    public KeyCode keyToPress;          // Tangenten som ska tryckas för att skylten ska försvinna

    private bool hasBeenTriggered = false;  // För att visa texten bara första gången

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; // Markera att vi redan visat denna
            PromptManager.Instance.ShowTutorial(tutorialText, keyToPress);
        }
    }
}