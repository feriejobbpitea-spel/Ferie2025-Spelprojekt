using UnityEngine;

public class ConfettiBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public float lifetime = 3f;
    public float explosionRadius = 0.5f;
    public LayerMask damageLayers;     // Vad som kan ta skada (Enemies, Player)
    public LayerMask collisionLayers;  // Vad som får bollen att explodera (Ground, Walls, Enemies)
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

        // Raycast i rörelseriktningen för att kolla om något i collisionLayers träffas
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, collisionLayers);

        if (hit.collider != null)
        {
            // Explodera oavsett vad vi träffar (mark, vägg, fiende)
            Explode();
        }

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
            Vector2 dirToTarget = hit.transform.position - transform.position;
            float distToTarget = dirToTarget.magnitude;

            // Kolla om något i collisionLayers blockerar sikten mellan explosion och mål
            RaycastHit2D obstacleCheck = Physics2D.Raycast(transform.position, dirToTarget.normalized, distToTarget, collisionLayers);

            if (obstacleCheck.collider != null && obstacleCheck.collider.gameObject != hit.gameObject)
            {
                // Skadan blockeras av en vägg eller annat objekt
                Debug.Log($"Skada blockerad av {obstacleCheck.collider.name} på väg till {hit.name}");
                continue;
            }

            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                var enemyHealth = hit.GetComponent<EnemyHealth>() ?? hit.GetComponentInChildren<EnemyHealth>();
                enemyHealth?.TakeDamage(damage);
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                hit.GetComponent<PlayerHealthV2>()?.LoseLife();
            }
        }

        Destroy(gameObject);
    }
}
