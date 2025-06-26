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
                // Ist�llet f�r att f�rst�ra plattformen, kan vi d�lja den och inaktivera r�relsen
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

    // Publik metod f�r att respawna plattformen, kalla denna fr�n spelarens script n�r spelaren d�r
    public void RespawnPlatform()
    {
        rb.position = startPos;
        previousPosition = startPos;
        hasBeenActivated = false;
        player = null;
        gameObject.SetActive(true);
    }
}