using UnityEngine;

public class breakabledoor : MonoBehaviour
{
    // S�tt detta i Unity: Layer  Projectile
    public string projectileLayerName = "Projectile";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(projectileLayerName))
        {
            Debug.Log("Block tr�ffades av projektil! G�r s�nder.");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}


