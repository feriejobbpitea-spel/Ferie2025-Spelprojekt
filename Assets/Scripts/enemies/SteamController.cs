using UnityEngine;
public class SteamController : MonoBehaviour
{
    public float activeTime = 2f;
    public float inactiveTime = 2f;

    private Collider2D steamCollider;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        steamCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(SteamCycle());
    }

    System.Collections.IEnumerator SteamCycle()
    {
        while (true)
        {
            // Ånga aktiv
            steamCollider.enabled = true;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            yield return new WaitForSeconds(activeTime);

            // Ånga inaktiv
            steamCollider.enabled = false;
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }
}