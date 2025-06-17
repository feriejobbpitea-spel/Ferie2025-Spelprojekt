using UnityEngine;

public class Movement : MonoBehaviour
{
    public float playerSpeed;
    //hopp
    public float jumpForce;
    public float jumpCutMultiplier;
    //
    public Transform groundCheck;
    public float groundCheckRadius;
    public Transform wallCheckL; //kollar om vi kramar en vägg åt vänster
    public Transform wallCheckR; //kollar om vi kramar en vägg åt höger
    

    private Rigidbody2D rb;  // Referens till Rigidbody2D-komponenten
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    private bool isGrounded;
    private float isRunning = 1;

    private bool isHuggingWall = false; //kramar en vägg
    private float wallJumpLockTime = 0.2f; // hur länge du låser styrning efter vägghopp
    private float wallJumpTimer = 0f;      // nedräkning
    private enum WallSide { None, Left, Right }
    private WallSide lastWallJumpSide = WallSide.None;
    private WallSide currentWallSide = WallSide.None;
    private float wallJumpXMomentum = 0.5f;
    private bool IsFacingWall()
    {
        return (currentWallSide == WallSide.Left && !facingRight) ||
               (currentWallSide == WallSide.Right && facingRight);
    }



    private bool walljumpUsed = false; //har använt vägghoppet
    //titta rätt håll
    private SpriteRenderer sr;
    private bool facingRight = true;
    //
   
    #region powerups
    private bool doubleJump = false;
    private bool doubleJumpUsed = false;
    private bool bigJump = false;
    public float bigJumpForce;
    private float superSpeed = 1; // vid 1 har man inte, 2 har man
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
        if (Input.GetKey(KeyCode.LeftShift)) {  isRunning = 2;  } else { isRunning = 1; }
        //

        float moveX = Input.GetAxis("Horizontal"); // bestämmer om du trycker a/d eller pil vänster höger

        Vector2 movement;

        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
            // Håll fast vid tidigare hopp-riktning medan kontroll är låst
            movement = new Vector2(wallJumpXMomentum, rb.linearVelocity.y);
        }
        else
        {
            wallJumpXMomentum = 0; // Rensa efter kontrollen återställs
            float moveDX = Input.GetAxis("Horizontal");
            float targetX = moveX * playerSpeed * isRunning * superSpeed;
            float smoothedX = Mathf.Lerp(rb.linearVelocity.x, targetX, 0.1f);
            movement = new Vector2(smoothedX, rb.linearVelocity.y);
        }



        // Hopp
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                movement.y = bigJump ? bigJumpForce : jumpForce;
            }
            else if (doubleJump && !doubleJumpUsed)
            {
                movement.y = bigJump ? bigJumpForce : jumpForce;
                doubleJumpUsed = true;
            }
            else if (isHuggingWall && currentWallSide != lastWallJumpSide && IsFacingWall())
            {
                // Spara senaste väggen
                lastWallJumpSide = currentWallSide;

                // Blockera input kort stund
                wallJumpTimer = wallJumpLockTime;

                // Tryck bort från väggen
                float direction = (currentWallSide == WallSide.Left) ? 1f : -1f;
                float xForce = direction * playerSpeed * 1.5f;  // TWEAKA styrka här!
                float yForce = jumpForce*4/5;

                // Direkt sätt velocity
                rb.linearVelocity = new Vector2(xForce, yForce);

                // Kom ihåg riktningen vi hoppade
                wallJumpXMomentum = xForce;

                return; // Stoppa movement denna frame
            }
        }

        //hopp, doublejump, bigjump
        rb.linearVelocity = movement;
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);// om spelaren är på marken eller inte



        // spelare tittar rätt håll 
        if (moveX < 0 && facingRight)
        {
            Flip();
        }
        else if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        
        
        bool huggingLeftWall = Physics2D.OverlapCircle(wallCheckL.position, groundCheckRadius, wallLayer); // om vi nuddar en vägg åt vänster
        bool huggingRightWall = Physics2D.OverlapCircle(wallCheckR.position, groundCheckRadius, wallLayer); // om vi nuddar en vägg åt höger


        if (huggingLeftWall)
{
    isHuggingWall = true;
    currentWallSide = WallSide.Left;
}
else if (huggingRightWall)
{
    isHuggingWall = true;
    currentWallSide = WallSide.Right;
}
else
{
    isHuggingWall = false;
    currentWallSide = WallSide.None;
}

      


        // Wall slide – endast om man tittar mot väggen
        if (isHuggingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            bool lookingAtWall = (currentWallSide == WallSide.Left && !facingRight) ||
                                 (currentWallSide == WallSide.Right && facingRight);

            if (lookingAtWall)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f); // Mjuk glidning
            }
        }



        if (isGrounded) { doubleJumpUsed = false; walljumpUsed = false;
            lastWallJumpSide = WallSide.None;
        }  //doublejump// Nollställ senaste vägghoppsvägg – så du kan hoppa på samma vägg igen

        #region Powerups
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0.3f;  // Slow motion
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Anpassa fysik
            }
            else
            {
                Time.timeScale = 1f;    // Normal fart
                Time.fixedDeltaTime = 0.02f; // Återställ fysik
            }
        }






        #endregion

    }
    void Flip()
    {
        facingRight = !facingRight;
        sr.flipX = !sr.flipX;
    }

}
