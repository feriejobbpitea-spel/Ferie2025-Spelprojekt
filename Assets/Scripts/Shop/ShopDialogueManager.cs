using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ShopDialogueManager : Singleton<ShopDialogueManager>
{
    public TMP_Text NameOfPerson;
    public TMP_Text Dialogue;
    public AudioSource Speaker;
    public Menu DialogueMenu; // Assume this has CanvasGroup for fade

    public float enterDuration = 0.5f;
    public float exitDuration = 0.5f;
    public float typingSpeed = 0.05f; // seconds per char

    public event System.Action OnDialogueStarted, OnDialogueStopped;

    private Coroutine typingCoroutine;

    // Call to start a new dialogue
    public void NewDialogue(string name, string dialogue, AudioClip voiceClip)
    {
        OnDialogueStarted?.Invoke();

        NameOfPerson.text = name;
        Dialogue.text = "";
        Speaker.Stop();
        Speaker.clip = voiceClip;

        ShowMenu(() =>
        {
            Speaker.Play();
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(dialogue, voiceClip != null ? voiceClip.length : 4f));
        });
    }

    private void ShowMenu(System.Action onComplete)
    {
        DialogueMenu.ShowMenu();
        CanvasGroup cg = DialogueMenu.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = DialogueMenu.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0;
        }
        cg.DOKill();
        cg.alpha = 0;
        cg.DOFade(1, enterDuration).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
    }

    private IEnumerator TypeText(string fullText, float clipLength)
    {
        Dialogue.text = "";
        float timer = 0f;
        int charIndex = 0;
        int totalChars = fullText.Length;

        while (charIndex < totalChars)
        {
            Dialogue.text += fullText[charIndex];
            charIndex++;

            timer += typingSpeed;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // Ensure full text is displayed after typing
        Dialogue.text = fullText;

        // Wait remaining time of clip after typing finishes
        float remaining = clipLength - (typingSpeed * totalChars);
        if (remaining > 0)
            yield return new WaitForSecondsRealtime(remaining);

        ClearDialogue();
    }

    public void ClearDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        Speaker.Stop();

        CanvasGroup cg = DialogueMenu.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.DOKill();
            cg.DOFade(0, exitDuration).OnComplete(() =>
            {
                DialogueMenu.HideMenu();
                Dialogue.text = "";
                NameOfPerson.text = "";
                OnDialogueStopped?.Invoke();
            });
        }
        else
        {
            DialogueMenu.HideMenu();
            Dialogue.text = "";
            NameOfPerson.text = "";
            OnDialogueStopped?.Invoke();
        }
    }
}
