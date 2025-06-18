using UnityEngine;

public class breakabledoor : MonoBehaviour
{
    // Sätt detta i Unity: Layer  Projectile
    public string projectileLayerName = "Projectile";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(projectileLayerName))
        {
            Debug.Log("Block träffades av projektil! Går sönder.");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}


