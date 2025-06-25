using UnityEngine;
public class PlayerLadderClimb : MonoBehaviour
{
    public float climbSpeed = 4f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private bool isClimbing = false;
    private float inputVertical;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        inputVertical = Input.GetAxisRaw("Vertical");

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, inputVertical * climbSpeed);
            rb.gravityScale = 0f;

            if (Input.GetButtonDown("Jump"))
            {
                ExitLadder();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ExitLadder();
        }
    }

    void ExitLadder()
    {
        isClimbing = false;
        rb.gravityScale = originalGravity;
    }
}
