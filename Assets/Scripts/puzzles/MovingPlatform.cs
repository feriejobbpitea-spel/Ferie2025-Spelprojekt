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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
                // Istället för att förstöra plattformen, kan vi dölja den och inaktivera rörelsen
                gameObject.SetActive(false);
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

    // Publik metod för att respawna plattformen, kalla denna från spelarens script när spelaren dör
    public void RespawnPlatform()
    {
        rb.position = startPos;
        previousPosition = startPos;
        hasBeenActivated = false;
        player = null;
        gameObject.SetActive(true);
    }
}