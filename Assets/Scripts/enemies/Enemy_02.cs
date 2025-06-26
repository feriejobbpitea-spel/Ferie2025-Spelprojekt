using UnityEngine;
public class Enemy_02 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 10f;
    public float aggroRange = 10f;

    public AudioSource shootAudioSource;   // Lägg till denna rad
    public AudioClip shootSound;            // Lägg till denna rad

    private float timer = 0f;
    private Transform player;
    public Transform healthBar;
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

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (direction != 0)
        {
            transform.localScale = new Vector3(
                -1 * Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
            healthBar.localScale = new Vector3(
                -1 * Mathf.Sign(direction) * Mathf.Abs(healthBar.localScale.x),
                healthBar.localScale.y,
                healthBar.localScale.z
            );
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= aggroRange)
        {
            timer += Time.deltaTime;
            if (timer >= shootInterval)
            {
                ShootAtPlayer();
                timer = 0f;
            }
        }

        if (stunTimer > 0f)
        {
            stunned = true;
            stunTimer -= Time.fixedDeltaTime;

            return;
        }
        else { stunned = false; }
    }
    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        Debug.Log($"Fienden är stunad i {duration} sekunder.");
        // Lägg till animation/effekt här vid behov
    }
    void ShootAtPlayer()
    {
        if (!stunned)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            projectile02 projScript = proj.GetComponent<projectile02>();
            projScript.SetDirection(direction);
            projScript.speed = projectileSpeed;

            // Spela skjutljudet
            if (shootAudioSource != null && shootSound != null)
            {
                shootAudioSource.PlayOneShot(shootSound);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
