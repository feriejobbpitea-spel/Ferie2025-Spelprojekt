using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private Image fadeImage; // Dra in din UI Image (hel skärm) i inspector
    [SerializeField] private float fadeDuration = 1f;

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        fadeImage.enabled = true;
        // Fade ut
        yield return StartCoroutine(Fade(0f, 1f));

        // Ladda scen asynkront
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }


        // Fade in
        yield return StartCoroutine(Fade(1f, 0f));
        fadeImage.enabled = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
