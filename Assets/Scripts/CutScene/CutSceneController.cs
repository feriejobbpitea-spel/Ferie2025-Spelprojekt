using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Localization.Settings;
using System.Collections;

[System.Serializable]
public class CutsceneSlide
{
    public Sprite image;
    [Tooltip("Nyckel till lokaliserad text i din Localization Table")]
    public string localizationKey;
}

public class CutSceneController : MonoBehaviour
{
    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;
    public TextFader textFader;
    public CanvasGroup continueText;
    public CanvasGroup skipText;
    public Image skipProgressBar;

    private int currentSlideIndex = 0;
    private bool isFading = false;

    private float skipHoldTime = 5f;
    private float holdTimer = 0f;

    private LocalizedString localizedString;
    private Coroutine updateTextCoroutine;

    IEnumerator Start()
    {
        localizedString = new LocalizedString();
        localizedString.TableReference = "Cut Scene Dialog";

        yield return LocalizationSettings.InitializationOperation;

        Debug.Log("Current Locale: " + LocalizationSettings.SelectedLocale.Identifier.Code);

        ShowSlide(currentSlideIndex);

        if (continueText != null) continueText.alpha = 0f;
        if (skipText != null) skipText.alpha = 1f;
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        StartCoroutine(InitialFadeIn());
    }

    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Return) && !isFading)
        {
            currentSlideIndex++;
            if (currentSlideIndex < slides.Length)
            {
                StartCoroutine(TransitionToSlide(currentSlideIndex));
            }
            else
            {
                SceneLoader.Instance.LoadScene("MainGame");
            }
        }
    }

    void ShowSlide(int index)
    {
        cutsceneImage.sprite = slides[index].image;

        localizedString.TableEntryReference = slides[index].localizationKey;

        Debug.Log($"Loading localization key: {localizedString.TableEntryReference} from table: {localizedString.TableReference}");

        if (updateTextCoroutine != null)
            StopCoroutine(updateTextCoroutine);

        updateTextCoroutine = StartCoroutine(UpdateLocalizedText());
    }

    private IEnumerator UpdateLocalizedText()
    {
        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Localized text: " + handle.Result);
            cutsceneText.text = handle.Result;
        }
        else
        {
            Debug.LogError("Failed to load localized string for key: " + localizedString.TableEntryReference);
            cutsceneText.text = "[Missing Text]";
        }
    }

    IEnumerator InitialFadeIn()
    {
        isFading = true;
        yield return textFader.FadeIn(3f);
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeInText(continueText, 1f));
        isFading = false;
    }

    IEnumerator TransitionToSlide(int index)
    {
        isFading = true;

        StartCoroutine(FadeOutText(continueText, 0.5f));

        yield return textFader.FadeOut(3f);
        ShowSlide(index);
        yield return textFader.FadeIn(3f);
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeInText(continueText, 1f));

        isFading = false;
    }

    IEnumerator FadeInText(CanvasGroup group, float duration)
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

    IEnumerator FadeOutText(CanvasGroup group, float duration)
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
        SceneLoader.Instance.LoadScene("MainGame");
    }
}
