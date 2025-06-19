using UnityEngine;
public class Enemy_01 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float aggroRange = 8f; // <-- Nytt: aggro range
    public LayerMask obstacleLayer;
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkRadius = 0.2f;
    public float wallCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Transform player;

    private bool isGrounded;
    private bool hittingWall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Rigidbody2D saknas på fienden!");

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Ingen spelare hittades med taggen 'Player'");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > aggroRange) return; // Utanför aggro: stå still

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, LayerMask.GetMask("Ground"));
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, obstacleLayer);

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (hittingWall && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Vänd fiendens sprite i rörelseriktning
        if (direction != 0)
        {
            transform.localScale = new Vector3(-1 * Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        // Ritning av ground och wall checks
        Gizmos.color = Color.green;
        if (wallCheck != null) Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        // Ritning av aggro-radie
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}












