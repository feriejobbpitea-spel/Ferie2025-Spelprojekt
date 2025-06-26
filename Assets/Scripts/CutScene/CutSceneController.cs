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
    public CutsceneSlide[] slides;
    public Image cutsceneImage;
    public TextMeshProUGUI cutsceneText;
    public TextFader textFader;
    public CanvasGroup continueText;
    public CanvasGroup skipText;
    public Image skipProgressBar;
    public AudioSource audioSource;

    public TextMeshProUGUI continueTextLabel;  // NY: Text-komponent för "Press ENTER to Continue"
    public TextMeshProUGUI skipTextLabel;      // NY: Text-komponent för "Hold ENTER to skip"

    private int currentSlideIndex = 0;
    private bool isFading = false;
    private bool canContinue = false;

    private float skipHoldTime = 2f;
    private float holdTimer = 0f;

    private LocalizedString localizedString;
    private Coroutine updateTextCoroutine;

    private KeyCode skipKey;  // Keybind för skip
    private KeyCode nextSlideKey; // Keybind för nästa slide

    IEnumerator Start()
    {
        localizedString = new LocalizedString();
        localizedString.TableReference = "Cut Scene Dialog";

        yield return LocalizationSettings.InitializationOperation;

        // Läs in keybinds från PlayerPrefs, fallback till Return
        string savedSkipKey = PlayerPrefs.GetString("bind_SkipCutscene", KeyCode.Return.ToString());
        if (!System.Enum.TryParse(savedSkipKey, out skipKey))
        {
            skipKey = KeyCode.Return;
        }

        string savedNextKey = PlayerPrefs.GetString("bind_NextSlide", KeyCode.Return.ToString());
        if (!System.Enum.TryParse(savedNextKey, out nextSlideKey))
        {
            nextSlideKey = KeyCode.Return;
        }

        // Uppdatera UI-texten med rätt knappnamn
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

        if (Input.GetKeyDown(nextSlideKey) && !isFading && canContinue)
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

    void UpdateContinueTextLabel()
    {
        if (continueTextLabel != null)
        {
            continueTextLabel.text = $"Press {KeyCodeToString(nextSlideKey)} to Continue";
        }
    }

    void UpdateSkipTextLabel()
    {
        if (skipTextLabel != null)
        {
            skipTextLabel.text = $"Hold {KeyCodeToString(skipKey)} to skip";
        }
    }

    string KeyCodeToString(KeyCode key)
    {
        // Anpassa sträng om du vill, t.ex. ersätt KeyCode.Return med "ENTER"
        if (key == KeyCode.Return) return "ENTER";
        if (key == KeyCode.Escape) return "ESC";
        // Lägg till fler specialfall vid behov

        return key.ToString().ToUpper();
    }

    // Resten av din kod är oförändrad, här är resten av klassen för referens:

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

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            cutsceneText.text = handle.Result;
        }
        else
        {
            cutsceneText.text = "[Missing Text]";
        }

        yield return new WaitForSeconds(0.2f);

        if (voiceClip != null)
        {
            var audioHandle = voiceClip.LoadAssetAsync();
            yield return audioHandle;

            if (audioHandle.Status == AsyncOperationStatus.Succeeded && audioHandle.Result != null)
            {
                audioSource.Stop();
                audioSource.clip = audioHandle.Result;
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying);
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
        SceneLoader.Instance.LoadScene("MainGame");
    }
}
