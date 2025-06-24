using UnityEngine;
public class SpiderWebSlow : MonoBehaviour
{
    public float slowMultiplier = 0.5f;
    public GameObject cover; // Covern som ska döljas

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Movement player = other.GetComponent<Movement>();
            if (player != null)
            {
                player.ApplySlow();
            }

            // Dölj cover om den finns
            if (cover != null)
            {
                cover.SetActive(false);
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