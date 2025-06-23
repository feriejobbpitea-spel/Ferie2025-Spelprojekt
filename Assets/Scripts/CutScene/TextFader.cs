using UnityEngine;
using TMPro;
using System.Collections;

public class TextFader : MonoBehaviour
{
    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();

        // Add a CanvasGroup component if it's missing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public IEnumerator FadeIn(float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            canvasGroup.alpha = timer / duration;
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        while (timer <= duration)
        {
            canvasGroup.alpha = 1 - (timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
