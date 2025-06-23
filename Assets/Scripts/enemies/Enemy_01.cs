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

    public float raycastDistance = 3f; // Max avst�nd ner�t f�r markkontroll

    private Rigidbody2D rb;
    private Transform player;

    private bool isGrounded;
    private bool hittingWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Rigidbody2D saknas p� fienden: " + gameObject.name);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Ingen spelare med taggen 'Player' hittades!");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Kolla mark under fienden
        isGrounded = false;
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, LayerMask.GetMask("Ground"));
        }
        else
        {
            Debug.LogError("groundCheck �r inte tilldelat i Inspector!");
        }

        // Kolla v�gg
        hittingWall = false;
        if (wallCheck != null)
        {
            hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, obstacleLayer);
        }
        else
        {
            Debug.LogError("wallCheck �r inte tilldelat i Inspector!");
        }

        float direction = 0f;

        // Kolla om spelaren �r n�ra nog f�r att aggera
        if (Vector2.Distance(player.position, transform.position) < aggroRange)
        {
            // R�kna ut riktning mot spelaren: +1 eller -1
            direction = Mathf.Sign(player.position.x - transform.position.x);

            // Raycast framifr�n och ned�t d�r fienden t�nker g�
            Vector2 rayOrigin = (Vector2)groundCheck.position + new Vector2(direction * 0.5f, 0);
            RaycastHit2D groundInfo = Physics2D.Raycast(rayOrigin, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));

            if (groundInfo.collider == null)
            {
                // Om det inte finns mark framf�r: stoppa (direction = 0)
                direction = 0f;
                Debug.Log("Fienden stannar pga stup framf�r");
            }
        }

        // S�tt velocity med linearVelocity
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Hoppa om hinder framf�r och st�r p� marken
        if (hittingWall && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // V�nd sprite i r�relseriktning
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
    }
    public Transform healthBar;
    private void OnDrawGizmos()
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
            Debug.LogWarning("wallCheck �r inte tilldelat � visar standardposition");
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        if (groundCheck != null)
        {
            // Rita raycast ner�t framf�r fienden
            float direction = 1f;
            if (player != null)
                direction = Mathf.Sign(player.position.x - transform.position.x);

            Vector2 rayOrigin = (Vector2)groundCheck.position + new Vector2(direction * 0.5f, 0);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * raycastDistance);
        }
    }
}