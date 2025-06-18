using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string tutorialText;         // Texten som ska visas
    public KeyCode keyToPress;          // Tangenten som ska tryckas f�r att skylten ska f�rsvinna

    private bool hasBeenTriggered = false;  // F�r att visa texten bara f�rsta g�ngen

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; // Markera att vi redan visat denna
            PromptManager.Instance.ShowTutorial(tutorialText, keyToPress);
        }
    }
}