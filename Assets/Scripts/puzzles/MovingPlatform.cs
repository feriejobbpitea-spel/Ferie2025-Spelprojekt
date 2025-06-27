using UnityEngine;
public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public float maxXPosition = 10f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 previousPosition;

    private GameObject player;
    private bool hasBeenActivated = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();

        startPos = rb.position;
        previousPosition = rb.position;
    }

    void FixedUpdate()
    {
        if (hasBeenActivated)
        {
            Vector2 movement = Vector2.right * speed * Time.fixedDeltaTime;
            Vector2 newPosition = rb.position + movement;

            if (newPosition.x >= maxXPosition)
            {
                // Dölj plattformen och inaktivera rörelse
                DisablePlatform();
                return;
            }

            rb.MovePosition(newPosition);

            if (player != null)
            {
                player.transform.position += (Vector3)(rb.position - previousPosition);
            }

            previousPosition = rb.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasBeenActivated)
            {
                hasBeenActivated = true;
                previousPosition = rb.position;
            }

            player = collision.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void DisablePlatform()
    {
        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        hasBeenActivated = false;
    }
    void OnEnable()
    {
        PlayerRespawn.OnPlayerRespawn += RespawnPlatform;
    }

    void OnDisable()
    {
        PlayerRespawn.OnPlayerRespawn -= RespawnPlatform;
    }


    // Publik metod för att återställa plattformen, anropas när spelaren dör
    public void RespawnPlatform()
    {
        Debug.Log("Plattform respawnar");

        rb.position = startPos;
        previousPosition = startPos;
        player = null;

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        hasBeenActivated = false;
    }
}