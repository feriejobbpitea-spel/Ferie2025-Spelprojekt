using UnityEngine;
using System.Collections;

public class ChangeBackgroundTrigger : MonoBehaviour
{
    public SpriteRenderer hide;
    public Color NewBackgroundColor;
    public GameObject[] ToEnable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeOut(hide, 1f)); // Starta Coroutine för fade-out
            foreach (var item in ToEnable)
            {
                if (item != null)
                {
                    item.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (var item in ToEnable)
            {
                if (item != null)
                {
                    item.SetActive(false);
                }
            }

           
        }
    }

    IEnumerator FadeOut(SpriteRenderer sr, float duration)
    {
        if (sr == null) yield break;

        float elapsed = 0f;
        Color c = sr.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = c;
            yield return null;
        }

        c.a = 0f;
        sr.color = c;
        sr.enabled = false;  // Dölj helt när fade är klar
    }
}
