using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float playerSpeed;
    public float jumpForce;
    public float jumpCutMultiplier;
    public Transform groundCheck;
    public float groundCheckRadius;
    public Transform wallCheckL;
    public Transform wallCheckR;
    public SpriteRenderer gfx;
    private Rigidbody2D rb;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public bool isGrounded;
    private float isRunning = 1;
    private bool wasGrabbingWall = false;
    private float wallJumpLockTime = 0.2f;
    private float wallJumpTimer = 0f;
    private float wallJumpXMomentum = 0.5f;
    public bool isGrabingwall = false;

    public event Action OnJump;
    private Tween fallStretchTween;

    public bool IsGrounded => isGrounded;
    public bool IsMoving => Input.GetAxisRaw("Horizontal") != 0;
    public float GetMoveSpeed => playerSpeed * isRunning * superSpeed;

    public bool facingRight = true;

    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    #region powerups
    private bool doubleJump = true;
    private bool doubleJumpUsed = false;
    private bool bigJump = false;
    public float bigJumpForce;
    private float superSpeed = 1;
    private bool timeSlow = false;
    #endregion

    // 🕸 Spiderweb slow support
    private int slowCounter = 0;
    private float normalSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalSpeed = playerSpeed; // 🕸 Save original speed
                                   // Ladda keybinds från PlayerPrefs
        keybinds["Jump"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Jump", KeyCode.Space.ToString()));
        keybinds["Sprint"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Sprint", KeyCode.LeftShift.ToString()));
        keybinds["Shoot"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Traps"))
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 4 / 5);
            
        }
    }
    public float currentCharge = 1f;
    public float maxCharge = 1f;
    public float rechargeRate = 0.1f; // Långsammare laddning

    void Update()
    {
        if (currentCharge < maxCharge)
        {
            currentCharge = Mathf.Min(currentCharge + rechargeRate * Time.deltaTime, maxCharge);
        }
        ApplyFallStretch();

        if (Input.GetKey(keybinds["Sprint"])) { if (isGrounded) { isRunning = 2; } } else { if (isGrounded) { isRunning = 1; } }

        keybinds["Left"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Left", KeyCode.A.ToString()));
        keybinds["Right"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Right", KeyCode.D.ToString()));

        float moveX = 0f;
        if (Input.GetKey(keybinds["Left"])) moveX = -1f;
        if (Input.GetKey(keybinds["Right"])) moveX = 1f;

        Vector2 movement;

        if (wallJumpTimer > 0)
        {
            wallJumpTimer -= Time.deltaTime;
            movement = new Vector2(wallJumpXMomentum, rb.linearVelocity.y);
        }
        else
        {
            wallJumpXMomentum = 0;
          
            if (Input.GetKey(keybinds["Left"])) moveX = -1f;
            if (Input.GetKey(keybinds["Right"])) moveX = 1f;
            
            float targetX = moveX * playerSpeed * isRunning * superSpeed;
            float smoothedX = Mathf.Lerp(rb.linearVelocity.x, targetX, 0.1f);
            movement = new Vector2(smoothedX, rb.linearVelocity.y);
        }

        if (Input.GetKeyDown(keybinds["Jump"]))
        {
            if (isGrounded)
            {
                OnJump?.Invoke();
                PlayJumpTween();
                movement.y = bigJump ? bigJumpForce : jumpForce;
            }
            else if (doubleJump && !doubleJumpUsed)
            {
                OnJump?.Invoke();
                PlayJumpTween();
                movement.y = bigJump ? bigJumpForce : jumpForce;
                doubleJumpUsed = true;
            }
            else if (isGrabingwall)
            {
                OnJump?.Invoke();
                PlayJumpTween();
                wallJumpTimer = wallJumpLockTime;
                float direction = (!facingRight) ? 1f : -1f;
                float xForce = direction * playerSpeed * 1.5f;
                float yForce = jumpForce * 4 / 5;
                rb.linearVelocity = new Vector2(xForce, yForce);
                wallJumpXMomentum = xForce;
                return;
            }
        }

        rb.linearVelocity = movement;

        if (Input.GetKeyUp(keybinds["Jump"]) && rb.linearVelocity.y > 0)

        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (moveX < 0 && facingRight) Flip();
        else if (moveX > 0 && !facingRight) Flip();

        bool huggingLeftWall = Physics2D.OverlapCircle(wallCheckL.position, groundCheckRadius, wallLayer);
        bool huggingRightWall = Physics2D.OverlapCircle(wallCheckR.position, groundCheckRadius, wallLayer);
        isGrabingwall = ((huggingLeftWall && !facingRight) || (huggingRightWall && facingRight));
        if (isGrabingwall && !wasGrabbingWall) PlayWallImpactTween();
        wasGrabbingWall = isGrabingwall;

        if (isGrabingwall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
        }

        if (isGrounded)
        {
            doubleJumpUsed = false;
            ResetScale();
        }

        #region Powerups
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0.3f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
        #endregion
        // Tryck på S för att sluta väggglida
        if (isGrabingwall && Input.GetKeyDown(KeyCode.S))
        {
            isGrabingwall = false;
            gfx.flipX = false;  // Titta rakt fram
            facingRight = !facingRight; // Synka variabeln
        }

    }
    
    // 🕸 Dessa två metoder behövs för spindelnätet
    public void ApplySlow()
    {
        slowCounter++;
        playerSpeed = normalSpeed * 0.5f; // Du kan justera detta
    }

    public void RemoveSlow()
    {
        slowCounter--;
        if (slowCounter <= 0)
        {
            slowCounter = 0;
            playerSpeed = normalSpeed;
        }
    }

    void PlayTurnTween()
    {
        gfx.transform.DOKill();
        Sequence turnSquash = DOTween.Sequence();
        turnSquash.Append(gfx.transform.DOScaleX(1.4f, 0.05f));
        turnSquash.Join(gfx.transform.DOScaleY(0.8f, 0.05f));
        turnSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void PlayWallImpactTween()
    {
        gfx.transform.DOKill();
        Sequence wallSquash = DOTween.Sequence();
        wallSquash.Append(gfx.transform.DOScaleX(0.7f, 0.05f));
        wallSquash.Join(gfx.transform.DOScaleY(1.3f, 0.05f));
        wallSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void PlayJumpTween()
    {
        gfx.transform.DOKill();
        Sequence jumpSquash = DOTween.Sequence();
        jumpSquash.Append(gfx.transform.DOScaleY(0.7f, 0.05f));
        jumpSquash.Join(gfx.transform.DOScaleX(1.3f, 0.05f));
        jumpSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void Flip()
    {
        facingRight = !facingRight;
        gfx.flipX = !facingRight;
        PlayTurnTween();
    }

    void ResetScale()
    {
        gfx.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
    }

    void ApplyFallStretch()
    {
        if (rb.linearVelocity.y < -0.1f && !isGrabingwall && !isGrounded)
        {
            if (fallStretchTween == null || !fallStretchTween.IsActive() || !fallStretchTween.IsPlaying())
            {
                fallStretchTween?.Kill();
                fallStretchTween = gfx.transform.DOScale(new Vector3(0.6f, 1.4f, 1f), 3f).SetEase(Ease.OutQuad);
            }
        }
        else
        {
            if (fallStretchTween != null && fallStretchTween.IsActive())
            {
                fallStretchTween.Kill();
                fallStretchTween = gfx.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
            }
        }
    }

}