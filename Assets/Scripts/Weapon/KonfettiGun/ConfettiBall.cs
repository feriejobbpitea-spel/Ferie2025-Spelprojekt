using UnityEngine;

public class ConfettiBall : MonoBehaviour
{
    public GameObject explosionEffect;      // Partikeleffekt
    public float lifetime = 3f;             // Max tid innan den exploderar
    public float explosionRadius = 0.5f;      // Radius för explosionen
    public LayerMask damageLayers;          // Vad som kan ta skada
    public int damage = 1;

    private bool exploded = false;

    private void Start()
    {
        Debug.Log("Confetti ball created!");
        Invoke(nameof(Explode), lifetime);  // Explodera automatiskt efter 'lifetime'
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (exploded) return;

        // Kolla om den träffar Ground eller Enemy
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("Confetti ball exploded!");

        if (explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Skada alla inom explosionens radie
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                hit.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.LoseLife();
            }


        }

        Destroy(gameObject);  // Förstör konfettibollen
    }
}
