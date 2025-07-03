using UnityEngine;

public class SpiderWebSlow : MonoBehaviour
{
    public float slowMultiplier = 0.5f;
    public GameObject cover; // Covern som ska döljas
    public AudioSource webSound; // Ljudet som spelas när spelaren går in i nätet

    private bool hasPlayedSound = false; // Flagga för att kolla om ljudet redan spelats

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.ApplySlow();
            }

            if (cover != null)
            {
                cover.SetActive(false);
            }

            // Spela ljud om det inte redan har spelats
            if (!hasPlayedSound && webSound != null)
            {
                webSound.Play();
                hasPlayedSound = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.RemoveSlow();
            }
        }
    }
}
