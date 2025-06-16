using UnityEngine;

public class Movment : MonoBehaviour
{
    public float playerSpeed;

    public float jumpForce;

    public Transform groundCheck;
    public float groundCheckRadius;

    private Rigidbody2D rb;  // Referens till Rigidbody2D-komponenten
    public LayerMask groundLayer;
    private bool isGrounded;
    private float isRunning = 1;

    //titta r�tt h�ll
    private SpriteRenderer sr;
    private bool facingRight = true;
    //
    //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {

        //spring
        if (Input.GetKey(KeyCode.LeftShift)) {  isRunning = 2; Debug.Log("running" + isRunning); } else { isRunning = 1; }


        float moveX = Input.GetAxis("Horizontal"); // best�mmer om du trycker a/d eller pil v�nster h�ger

        Vector2 movement = new Vector2(moveX * playerSpeed * isRunning, rb.linearVelocity.y);
        // spelare tittar r�tt h�ll 
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }


        // Kolla om vi st�r p� marken
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Hopp
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            movement.y=jumpForce;
        }






        print(movement);
        rb.linearVelocity = movement;
    }
    void Flip()
    {
        facingRight = !facingRight;
        sr.flipX = !sr.flipX;
    }

}
