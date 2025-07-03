using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SteamController : MonoBehaviour
{
    public float activeTime = 2f;
    public float inactiveTime = 2f;

    private Collider2D steamCollider;
    private SpriteRenderer spriteRenderer;
    private AudioSource steamAudio;

    void Start()
    {
        steamCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        steamAudio = GetComponent<AudioSource>();

        if (steamAudio != null)
        {
            steamAudio.loop = true; // Så att ljudet loopas under ångperioden
        }

        StartCoroutine(SteamCycle());
    }

    System.Collections.IEnumerator SteamCycle()
    {
        while (true)
        {
            // Ånga aktiv
            steamCollider.enabled = true;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (steamAudio != null && !steamAudio.isPlaying) steamAudio.Play();

            yield return new WaitForSeconds(activeTime);

            // Ånga inaktiv
            steamCollider.enabled = false;
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (steamAudio != null && steamAudio.isPlaying) steamAudio.Stop();

            yield return new WaitForSeconds(inactiveTime);
        }
    }
}
