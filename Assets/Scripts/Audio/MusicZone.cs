using UnityEngine;
using System.Collections;
public class MusicZone : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Inställningar")]
    [Tooltip("Målvolym när spelaren går in i zonen (0 = tyst, 1 = max)")]
    [Range(0f, 1f)]
    public float targetVolume = 0.2f;

    [Tooltip("Hur lång tid det tar att fada in/ut (i sekunder)")]
    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Starta tyst
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            if (!audioSource.isPlaying)
                audioSource.Play();

            fadeCoroutine = StartCoroutine(FadeAudio(targetVolume)); // Fada in till inställd volym
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutAndStop());
        }
    }

    private IEnumerator FadeAudio(float target)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, target, time / fadeDuration);
            yield return null;
        }

        audioSource.volume = target;
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return StartCoroutine(FadeAudio(0f));
        audioSource.Stop();
    }
}