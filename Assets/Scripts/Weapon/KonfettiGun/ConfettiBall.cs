using UnityEngine;

public class ConfettiBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public float lifetime = 3f;
    public float explosionRadius = 0.5f;
    public LayerMask damageLayers;     // Vad som kan ta skada (Enemies, Player)
    public LayerMask collisionLayers;  // Vad som får bollen att explodera (Ground, Enemies)
    public int damage = 1;

    private bool exploded = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke(nameof(Explode), lifetime);
    }

    void FixedUpdate()
    {
        if (exploded || rb == null) return;

        Vector2 direction = rb.linearVelocity.normalized;
        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime + 0.05f;

        // Raycast i rörelseriktningen
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, collisionLayers);

        if (hit.collider != null)
        {
            Explode();
        }

        // (Valfritt) Debug-linje i Scene View:
        Debug.DrawRay(transform.position, direction * distance, Color.red);
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                if(hit.name == "Boss")
                {
                   hit.GetComponent<Boss>()?.TakeDamage(damage);
                } else { hit.GetComponent<EnemyHealth>()?.TakeDamage(damage); }
                    
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hit.GetComponent<PlayerHealthV2>()?.LoseLife();
            }
        }

        Destroy(gameObject);
    }
}
