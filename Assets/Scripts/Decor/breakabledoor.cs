using UnityEngine;

public class breakabledoor : MonoBehaviour
{
    // S�tt detta i Unity: Layer  Projectile
    public string projectileLayerName = "Projectiles";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // L�gg till denna rad f�rst:
        Debug.Log("N�got gick in i triggern!");

        if (collision.gameObject.layer == LayerMask.NameToLayer(projectileLayerName))
        {
            Debug.Log("Block tr�ffades av projektil! G�r s�nder.");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}






