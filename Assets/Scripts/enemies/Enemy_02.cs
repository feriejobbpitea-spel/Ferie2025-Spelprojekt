using UnityEngine;
public class Enemy_02 : MonoBehaviour
{
    [Header("Skjutinställningar")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 10f;
    public float aggroRange = 10f;

    [Header("Ljud")]
    public AudioSource shootAudioSource;
    public AudioClip shootSound;

    [Header("Siktinställningar")]
    public LayerMask visionMask; // Inkludera Player och Steam i denna

    [Header("Referenser")]
    public Transform healthBar;

    private float timer = 0f;
    private Transform player;
    private float stunTimer = 0f;
    private bool stunned = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found in scene!");
        }

        if (shootAudioSource == null)
        {
            shootAudioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Stun-hantering
        if (stunTimer > 0f)
        {
            stunned = true;
            stunTimer -= Time.deltaTime;
            return;
        }
        else
        {
            stunned = false;
        }

        // Vänd mot spelaren
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        if (direction != 0)
        {
            transform.localScale = new Vector3(
                -1 * direction * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
            healthBar.localScale = new Vector3(
                -1 * direction * Mathf.Abs(healthBar.localScale.x),
                healthBar.localScale.y,
                healthBar.localScale.z
            );
        }

        // Avstånd och siktkontroll
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= aggroRange && CanSeePlayer())
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                ShootAtPlayer();
                timer = 0f;
            }
        }

        // (Debug) Visa Raycast i scenen
        Debug.DrawRay(firePoint.position, (player.position - firePoint.position).normalized * aggroRange, Color.green);
    }

    /// <summary>
    /// Returnerar true om fienden har fri sikt till spelaren (dvs ånga inte blockerar).
    /// </summary>
    bool CanSeePlayer()
    {
        Vector2 direction = player.position - firePoint.position;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction.normalized, aggroRange, visionMask);

        if (hit.collider != null)
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    /// <summary>
    /// Stunnar fienden i angiven tid.
    /// </summary>
    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        Debug.Log($"Fienden är stunad i {duration} sekunder.");
    }

    /// <summary>
    /// Skjuter ett projektil mot spelaren.
    /// </summary>
    void ShootAtPlayer()
    {
        if (stunned || player == null) return;

        Vector3 direction = (player.position - firePoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile02 projScript = proj.GetComponent<projectile02>();
        projScript.SetDirection(direction);
        projScript.speed = projectileSpeed;

        if (shootAudioSource != null && shootSound != null)
        {
            shootAudioSource.PlayOneShot(shootSound);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}