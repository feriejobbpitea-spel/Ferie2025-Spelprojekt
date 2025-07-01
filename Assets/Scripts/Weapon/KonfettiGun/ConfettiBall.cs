using UnityEngine;

public class ConfettiBall : MonoBehaviour
{
    public GameObject explosionEffect;
    public float lifetime = 3f;
    public float explosionRadius = 0.5f;
    public LayerMask damageLayers;
    public LayerMask collisionLayers;
    public int damage = 1;

    public AudioClip shootLoopSound;
    public AudioClip explosionSound;

    private bool exploded = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lägg till AudioSource och spela loopande skottljud
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        if (shootLoopSound != null)
        {
            audioSource.clip = shootLoopSound;
            audioSource.Play();
        }

        Invoke(nameof(Explode), lifetime);
    }

    void FixedUpdate()
    {
        if (exploded || rb == null) return;

        Vector2 direction = rb.linearVelocity.normalized;
        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime + 0.05f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, collisionLayers);

        if (hit.collider != null)
        {
            Explode();
        }

        Debug.DrawRay(transform.position, direction * distance, Color.red);
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Stoppa rörelse och gör kinematisk
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Stäng av collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Dölj sprite renderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // Stoppa loopande ljud
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Skapa explosionseffekt
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Applicera skada
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);

        foreach (Collider2D hit in hits)
        {
            Vector2 dirToTarget = hit.transform.position - transform.position;
            float distToTarget = dirToTarget.magnitude;

            RaycastHit2D obstacleCheck = Physics2D.Raycast(transform.position, dirToTarget.normalized, distToTarget, collisionLayers);

            if (obstacleCheck.collider != null && obstacleCheck.collider.gameObject != hit.gameObject)
            {
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

        // Spela explosionsljud i separat GameObject
        if (explosionSound != null)
        {
            GameObject soundObj = new GameObject("ExplosionSound");
            soundObj.transform.position = transform.position;
            AudioSource soundSource = soundObj.AddComponent<AudioSource>();
            soundSource.clip = explosionSound;
            soundSource.Play();
            Destroy(soundObj, explosionSound.length);
        }

        // Förstör projektilen direkt
        Destroy(gameObject);
    }
}
