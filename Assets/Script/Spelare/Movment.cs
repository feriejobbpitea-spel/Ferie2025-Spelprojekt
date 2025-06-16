using UnityEngine;

public class Movment : MonoBehaviour
{
    public float playerSpeed;
    //hopp
    public float jumpForce;
    public float jumpCutMultiplier;
    //
    public Transform groundCheck;
    public float groundCheckRadius;

    private Rigidbody2D rb;  // Referens till Rigidbody2D-komponenten
    public LayerMask groundLayer;
    private bool isGrounded;
    private float isRunning = 1;

    //titta rätt håll
    private SpriteRenderer sr;
    private bool facingRight = true;
    //
   
    #region powerups
    private bool doubleJump = false;
    private bool doubleJumpUsed = false;
    private bool bigJump = true;
    public float bigJumpForce;
    private bool superSpeed = false;
    private bool timeSlow = false;
    #endregion

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
        //

        float moveX = Input.GetAxis("Horizontal"); // bestämmer om du trycker a/d eller pil vänster höger

        Vector2 movement = new Vector2(moveX * playerSpeed * isRunning, rb.linearVelocity.y);
       
        // Hopp
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) //hoppar aningen om man är på marken eller har powerup
        {
            if (bigJump) { movement.y = bigJumpForce; }
            else { movement.y = jumpForce; }
            
        } else if(Input.GetKeyDown(KeyCode.Space) && doubleJump && !doubleJumpUsed) { doubleJumpUsed = true; if (bigJump) { movement.y = bigJumpForce; }
            else { movement.y = jumpForce; }
        } //hopp, doublejump, bigjump
            rb.linearVelocity = movement;
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        //

        // spelare tittar rätt håll 
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        //

        // Kolla om vi står på marken
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //

        #region Powerups
        if (isGrounded) {doubleJumpUsed = false;}  //doublejump
        





        #endregion

    }
    void Flip()
    {
        facingRight = !facingRight;
        sr.flipX = !sr.flipX;
    }

}
