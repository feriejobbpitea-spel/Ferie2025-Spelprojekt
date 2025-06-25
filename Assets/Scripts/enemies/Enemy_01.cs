using UnityEngine;

public class Enemy_01 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float aggroRange = 8f;
    public LayerMask obstacleLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    public float wallCheckRadius = 0.2f;
    public float raycastDistance = 3f;

    public Transform healthBar;

    private Rigidbody2D rb;
    private Transform player;

    private bool isGrounded;
    private bool hittingWall;

    private float stunTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Rigidbody2D saknas på fienden: " + gameObject.name);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Ingen spelare med taggen 'Player' hittades!");
    }

    void FixedUpdate()
    {
        if (stunTimer > 0f)
        {
            stunTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Fienden står still i x-led
            return;
        }

        if (player == null) return;

        isGrounded = groundCheck != null && Physics2D.OverlapCircle(groundCheck.position, checkRadius, LayerMask.GetMask("Ground"));
        hittingWall = wallCheck != null && Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, obstacleLayer);

        float direction = 0f;

        if (Vector2.Distance(player.position, transform.position) < aggroRange)
        {
            direction = Mathf.Sign(player.position.x - transform.position.x);

            Vector2 rayOrigin = (Vector2)groundCheck.position + new Vector2(direction * 0.5f, 0);
            LayerMask groundAndTrapMask = LayerMask.GetMask("Ground", "Traps");

            RaycastHit2D groundInfo = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDistance, groundAndTrapMask);

            if (groundInfo.collider != null)
            {
                int hitLayer = groundInfo.collider.gameObject.layer;
                if (hitLayer == LayerMask.NameToLayer("Traps"))
                {
                    direction = 0f;
                    Debug.Log("Fienden stannar pga trap framför");
                }
            }
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (hittingWall && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (direction != 0)
        {
            float dirSign = Mathf.Sign(direction);
            transform.localScale = new Vector3(-1 * dirSign * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (healthBar != null)
            {
                healthBar.localScale = new Vector3(-1 * dirSign * Mathf.Abs(healthBar.localScale.x), healthBar.localScale.y, healthBar.localScale.z);
            }
        }
    }

    

    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        Debug.Log($"Fienden är stunad i {duration} sekunder.");
        // Lägg till animation/effekt här vid behov
    }

    void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, wallCheckRadius);
            Debug.LogWarning("wallCheck är inte tilldelat – visar standardposition");
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        if (groundCheck != null)
        {
            float direction = 1f;
            if (player != null)
                direction = Mathf.Sign(player.position.x - transform.position.x);

            Vector2 rayOrigin = (Vector2)groundCheck.position + new Vector2(direction * 0.5f, 0);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * raycastDistance);
        }
    }
}
