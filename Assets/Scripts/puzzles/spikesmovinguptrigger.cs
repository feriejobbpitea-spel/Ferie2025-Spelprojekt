using UnityEngine;

public class spikesmovinguptrigger : MonoBehaviour
{
    public spikesmovingup platform; // Dra in plattformen i Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!platform.IsAtStartPosition())
            {
                platform.ResetPlatform();
            }
            platform.ActivatePlatform();
        }
    }
}


