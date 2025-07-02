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
    public string localizationKey;
    public LocalizedAudioClip voiceLine;
}

public class CutSceneController : MonoBehaviour
{
    [Header("Cutscene Content")]
    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;
    public TextFader textFader;

    [Header("UI Elements")]
    public CanvasGroup continueText;
    public CanvasGroup skipText;
    public Image skipProgressBar;

    public TextMeshProUGUI continueTextLabel;
    public TextMeshProUGUI skipTextLabel;

    public string TableReference = "Cut Scene Dialog"; // NY: Tabellreferens för lokalisering

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Scene Settings")]
    public string sceneToLoadAfterCutscene = "MainGame"; // NY: målscen efter cutscene

    private int currentSlideIndex = 0;
    private bool isFading = false;
    private bool canContinue = false;

    private float skipHoldTime = 2f;
    private float holdTimer = 0f;

    private LocalizedString localizedString;
    private Coroutine updateTextCoroutine;

    private KeyCode skipKey;
    private KeyCode nextSlideKey;

    IEnumerator Start()
    {
        localizedString = new LocalizedString();
        localizedString.TableReference = TableReference;

        yield return LocalizationSettings.InitializationOperation;

        string savedSkipKey = PlayerPrefs.GetString("bind_SkipCutscene", KeyCode.Return.ToString());
        if (!System.Enum.TryParse(savedSkipKey, out skipKey)) skipKey = KeyCode.Return;

        string savedNextKey = PlayerPrefs.GetString("bind_NextSlide", KeyCode.Return.ToString());
        if (!System.Enum.TryParse(savedNextKey, out nextSlideKey)) nextSlideKey = KeyCode.Return;

        UpdateContinueTextLabel();
        UpdateSkipTextLabel();

        ShowSlide(currentSlideIndex);

        if (continueText != null) continueText.alpha = 0f;
        if (skipText != null) skipText.alpha = 1f;
        if (skipProgressBar != null) skipProgressBar.fillAmount = 0f;

        StartCoroutine(InitialFadeIn());
    }

    void Update()
    {
        if (Input.GetKey(skipKey))
        {
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / skipHoldTime);
            if (skipProgressBar != null)
                skipProgressBar.fillAmount = progress;

            if (holdTimer >= skipHoldTime)
                SkipCutscene();
        }
        else
        {
            holdTimer = 0f;
            if (skipProgressBar != null)
                skipProgressBar.fillAmount = 0f;
        }

        if (Input.GetKeyDown(nextSlideKey) && !isFading && canContinue)
        {
            currentSlideIndex++;
            if (currentSlideIndex < slides.Length)
            {
                StartCoroutine(TransitionToSlide(currentSlideIndex));
            }
            else
            {
                SceneLoader.Instance.LoadScene(sceneToLoadAfterCutscene); // NYTT
            }
        }
    }

    void UpdateContinueTextLabel()
    {
        if (continueTextLabel != null)
            continueTextLabel.text = $"Press {KeyCodeToString(nextSlideKey)} to Continue";
    }

    void UpdateSkipTextLabel()
    {
        if (skipTextLabel != null)
            skipTextLabel.text = $"Hold {KeyCodeToString(skipKey)} to skip";
    }

    string KeyCodeToString(KeyCode key)
    {
        if (key == KeyCode.Return) return "ENTER";
        if (key == KeyCode.Escape) return "ESC";
        return key.ToString().ToUpper();
    }

    void ShowSlide(int index)
    {
        cutsceneImage.sprite = slides[index].image;
        localizedString.TableEntryReference = slides[index].localizationKey;

        if (updateTextCoroutine != null)
            StopCoroutine(updateTextCoroutine);

        updateTextCoroutine = StartCoroutine(UpdateLocalizedText(slides[index].voiceLine));
    }

    private IEnumerator UpdateLocalizedText(LocalizedAudioClip voiceClip)
    {
        canContinue = false;
        if (continueText != null) continueText.alpha = 0f;

        var handle = localizedString.GetLocalizedStringAsync();
        yield return handle;

        cutsceneText.text = handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : "[Missing Text]";
        yield return new WaitForSeconds(0.2f);

        if (voiceClip != null)
        {
            var audioHandle = voiceClip.LoadAssetAsync();
            yield return audioHandle;
            if (audioHandle.Status == AsyncOperationStatus.Succeeded && audioHandle.Result != null)
            {
                AudioClip clip = audioHandle.Result as AudioClip;
                if (clip == null)
                {
                    Debug.LogError("Loaded asset is not an AudioClip!");
                    yield break;
                }

                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying);
            }
            else
            {
                Debug.LogError("Failed to load AudioClip from localized voiceLine.");
            }
        }

        StartCoroutine(FadeInText(continueText, 1f));
        canContinue = true;
    }

    IEnumerator InitialFadeIn()
    {
        isFading = true;
        yield return textFader.FadeIn(1f);
        yield return new WaitForSeconds(1f);
        isFading = false;
    }

    IEnumerator TransitionToSlide(int index)
    {
        isFading = true;
        StartCoroutine(FadeOutText(continueText, 0.5f));
        yield return textFader.FadeOut(1f);

        ShowSlide(index);

        yield return textFader.FadeIn(2.5f);
        yield return new WaitForSeconds(0.5f);
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
        SceneLoader.Instance.LoadScene(sceneToLoadAfterCutscene); // NYTT
    }
}
