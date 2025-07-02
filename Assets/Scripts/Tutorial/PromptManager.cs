using UnityEngine;
using TMPro;
using DG.Tweening;

public class PromptManager : Singleton<PromptManager>
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialTextComponent;

    private bool isShowing = false;
    private KeyCode currentKeyToPress;

    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (tutorialPanel != null)
        {
            canvasGroup = tutorialPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = tutorialPanel.AddComponent<CanvasGroup>();
            }

            // Sätt initial alpha till 0 om det inte redan är det
            canvasGroup.alpha = 0;
        }
    }

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
        if (tutorialPanel != null && tutorialTextComponent != null && canvasGroup != null)
        {
            tutorialPanel.SetActive(true);
            tutorialTextComponent.text = text;
            currentKeyToPress = keyToPress;
            isShowing = true;

            // Fade in
            canvasGroup.alpha = 0f;
            tutorialPanel.transform.localScale = Vector3.zero;
            tutorialPanel.transform.DOScale(Vector3.one, fadeDuration).SetEase(Ease.OutBack);
            canvasGroup.DOFade(1f, fadeDuration);
        }
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null && canvasGroup != null)
        {
            // Fade out
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                tutorialPanel.SetActive(false);
                isShowing = false;
            });
        }
    }
}