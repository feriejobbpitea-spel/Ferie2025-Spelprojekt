using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CutsceneSlide
{
    public Sprite image;
    [TextArea(2, 5)]
    public string text;
}

public class CutSceneController : MonoBehaviour
{
    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;
    public TextFader textFader;
    public CanvasGroup continueText;
    public CanvasGroup skipText;
    public Image skipProgressBar; // <-- lägg till denna i Unity Inspector

    private int currentSlideIndex = 0;
    private bool isFading = false;

    private float skipHoldTime = 5f;
    private float holdTimer = 0f;

    void Start()
    {
        ShowSlide(currentSlideIndex);

        if (continueText != null) continueText.alpha = 0f;
        if (skipText != null) skipText.alpha = 1f; // alltid synlig
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        StartCoroutine(InitialFadeIn());
    }

    void Update()
    {
        // Håll Enter för att skippa
        if (Input.GetKey(KeyCode.Return))
        {
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / skipHoldTime);

            if (skipProgressBar != null)
                skipProgressBar.fillAmount = progress;

            if (holdTimer >= skipHoldTime)
            {
                SkipCutscene();
            }
        }
        else
        {
            holdTimer = 0f;
            if (skipProgressBar != null)
                skipProgressBar.fillAmount = 0f;
        }

        // Tryck Enter för att gå vidare (endast om inte håller ner)
        if (Input.GetKeyDown(KeyCode.Return) && !isFading)
        {
            currentSlideIndex++;
            if (currentSlideIndex < slides.Length)
            {
                StartCoroutine(TransitionToSlide(currentSlideIndex));
            }
            else
            {
                SceneManager.LoadScene("Level1");
            }
        }
    }

    void ShowSlide(int index)
    {
        cutsceneImage.sprite = slides[index].image;
        cutsceneText.text = slides[index].text;
    }

    System.Collections.IEnumerator InitialFadeIn()
    {
        isFading = true;
        yield return textFader.FadeIn(3f);
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeInText(continueText, 1f));
        // skipText syns alltid, vi skippar FadeIn

        isFading = false;
    }

    System.Collections.IEnumerator TransitionToSlide(int index)
    {
        isFading = true;

        StartCoroutine(FadeOutText(continueText, 0.5f));
        // skipText alltid synlig, vi skippar FadeOut

        yield return textFader.FadeOut(3f);
        ShowSlide(index);
        yield return textFader.FadeIn(3f);
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeInText(continueText, 1f));

        isFading = false;
    }

    System.Collections.IEnumerator FadeInText(CanvasGroup group, float duration)
    {
        if (group == null) yield break;

        float timer = 0f;
        while (timer < duration)
        {
            group.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        group.alpha = 1f;
    }

    System.Collections.IEnumerator FadeOutText(CanvasGroup group, float duration)
    {
        if (group == null) yield break;

        float timer = 0f;
        while (timer < duration)
        {
            group.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        group.alpha = 0f;
    }

    void SkipCutscene()
    {
        SceneManager.LoadScene("Level1");
    }
}
