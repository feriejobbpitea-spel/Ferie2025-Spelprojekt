using UnityEngine;

public class breakabledoor : MonoBehaviour
{
    // Sätt detta i Unity: Layer  Projectile
    public string projectileLayerName = "Projectiles";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Lägg till denna rad först:
        Debug.Log("Något gick in i triggern!");

        if (collision.gameObject.layer == LayerMask.NameToLayer(projectileLayerName))
        {
            Debug.Log("Block träffades av projektil! Går sönder.");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}






