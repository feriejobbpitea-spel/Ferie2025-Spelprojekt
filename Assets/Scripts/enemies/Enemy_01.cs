using UnityEngine;

public class Enemy_01 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public LayerMask obstacleLayer; // Layer f�r v�ggar/hinder
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
            Debug.LogError("Rigidbody2D saknas p� fienden: " + gameObject.name);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Ingen spelare med taggen 'Player' hittades!");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Kolla mark
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

        // R�relse mot spelaren
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Hoppa om hinder framf�r och st�r p� marken
        if (hittingWall && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // V�nd fiendens sprite i r�relseriktning
        if (direction != 0)
        {
            transform.localScale = new Vector3(-1 * Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x),
                                              transform.localScale.y,
                                              transform.localScale.z);
        }
    }

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
    }
}